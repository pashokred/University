from Pyro4 import expose

class Solver:
    def __init__(self, workers=None, input_file_name=None, output_file_name=None):
        self.input_file_name = input_file_name
        self.output_file_name = output_file_name
        self.workers = workers
        print("Inited")

    def solve(self):
        print("Job Started")
        print("Workers %d" % len(self.workers))

        data = self.read_input()

        step = len(data) / len(self.workers)
        # map
        mapped = []
        for i in range(0, len(self.workers)):
            workerdata = []
            row = i
            while row < len(data):
                workerdata.append(data[row])
                row += len(self.workers)

            mapped.append(self.workers[i].gaussFunc(workerdata))

        print('Map finished: ', mapped)

        # reduce
        reduced = self.myreduce(mapped)

        # output
        self.write_output(reduced)

        print("Job Finished")

    @staticmethod
    @expose
    def gaussFunc(a):

        c = [row[:] for row in a]

        len1 = len(a)
        len2 = len(a[0])
        for g in range(len1):

            max = abs(a[g][g])
            my = g
            t1 = g
            while t1 < len1:
                if abs(a[t1][g]) > max:
                    max = abs(a[t1][g])
                    my = t1
                t1 += 1

            if my != g:
                b = [el for el in a[g]]
                a[g] = [el for el in a[my]]
                a[my] = [el for el in b]
            if max == 0.0: continue
            amain = float(a[g][g])

            z = g
            while z < len2:
                a[g][z] = a[g][z] / amain
                z += 1

            j = g + 1

            while j < len1:
                b = a[j][g]
                z = g

                while z < len2:
                    a[j][z] = a[j][z] - a[g][z] * b
                    z += 1
                j += 1

        m = len1 - 1
        while m > 0:
            n = m - 1

            while n >= 0:
                a[n][len1] = a[n][len1] - a[n][m] * a[m][len1]
                n -= 1
            m -= 1
        res = []
        for el in a[:]:
            res.append(el[len2 - 1])
        return res

    @staticmethod
    @expose
    def myreduce(mapped):
        output = []
        for i in mapped:
            for j in i.value:
                output.append(j)
        return output

    def read_input(self):
        data = []
        with open(self.input_file_name) as f:
            for line in f:
                data.append([float(x) for x in line.split()])
        return data

    def write_output(self, output):
        f = open(self.output_file_name, 'w')
        f.write(str(output))
        f.write('\n')
        f.close()