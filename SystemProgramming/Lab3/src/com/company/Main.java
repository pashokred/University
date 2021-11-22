package com.company;

import java.io.File;
import java.io.FileNotFoundException;
import java.nio.file.Path;
import java.nio.file.Paths;
import java.util.Scanner;

public class Main {
    static Scanner sc = new Scanner(System.in);

    private static File readFile() {
        Scanner input = new Scanner(System.in);
        System.out.println("Please, enter file name");
        String filename = input.nextLine();

        Path path = Paths.get(filename);
        File file = path.toFile();
        input.close();
        return file;
    }

    public static void main(String[] args) {
        File file = readFile();

        try {
            Lexer lexer = new Lexer(file);
            System.out.println(lexer.toString());
        } catch (FileNotFoundException e) {
            System.out.println(e);
        }
    }
}
