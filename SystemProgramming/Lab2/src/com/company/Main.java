package com.company;

import java.io.FileReader;
import java.nio.file.Path;
import java.nio.file.Paths;
import java.util.Collection;
import java.util.Scanner;

public class Main {

    private static Path getFilePath() throws IllegalArgumentException {
        Scanner input = new Scanner(System.in);
        System.out.println("Please, enter file name");
        String filename = input.nextLine();
        input.close();

        return Paths.get(filename);
    }

    private static <T> void printCollection(Collection<T> collection) {
        for (var word : collection) {
            System.out.println(word);
        }
    }

    private static Automaton readAutomaton(FileReader file) {
        Scanner sc = new Scanner(file);
        final int nAlphabet = sc.nextInt();

        final int nState = sc.nextInt();
        final int startState = sc.nextInt();
        Automaton automaton = new Automaton(nState, startState, nAlphabet);

        final int nFinalState = sc.nextInt();
        for (int i = 0; i < nFinalState; ++i) {
            final int st = sc.nextInt();
            automaton.addFinalState(st);
        }

        while (sc.hasNext()) {
            final int curState = sc.nextInt();
            final int input = sc.next().codePointAt(0);
            final int nextState = sc.nextInt();
            automaton.addTransition(curState, input, nextState);
        }
        sc.close();
        return automaton;
    }

    public static void main(String[] args) {
        final Path filePath;
        try {
            filePath = getFilePath();
        } catch (IllegalArgumentException ex) {
            System.err.println(ex.getLocalizedMessage());
            System.exit(1);
            return;
        }

        try (FileReader fr = new FileReader(filePath.toString())) {
            final Automaton automaton = readAutomaton(fr);
            System.out.println("Dead states:");
            printCollection(automaton.deadStates());
            System.out.println("Unreachable states:");
            printCollection(automaton.unreachableStates());

            automaton.removeDeadStates();
            automaton.removeUnreachableStates();
            while (automaton.removeEqualStates()) {
                automaton.removeEqualStates();
            }

            System.out.println(automaton);
        } catch (Exception ex) {
            System.err.println(ex.getLocalizedMessage());
            System.exit(1);
            return;
        }
    }
}
