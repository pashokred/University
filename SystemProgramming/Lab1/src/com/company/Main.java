package com.company;

import java.io.IOException;
import java.nio.file.Files;
import java.nio.file.Paths;
import java.util.Arrays;
import java.util.HashSet;

// Редько Павло МІ-3
// Лаб 1, Варіант 4:
// Знайти лише ті слова, кожне з яких складається з літер що не повторюються.

public class Main {
    public static void main(String[] args) {
        String text = "", path = "src/com/company/DoctorWho_ BorrowedTimebyNaomiAlderman.txt",

                // this regex means that we consider words with ' (Peter's)
                // we don't consider digits as words (I didn't find it in requirements)
                // we consider words with different case different ("their" and "Their" is different)
                regex = "(?=[^a-zA-Z]+)[^'-]";

        try {
            text = new String(Files.readAllBytes(Paths.get(path)));
        }
        catch (IOException e) {
            e.printStackTrace();
        }

        Arrays.stream(text.split(regex))
                .filter(Main::hasAllUniqueChars)
                .distinct()
                .forEach(System.out::println);
    }

    public static boolean hasAllUniqueChars (String word) {
        var maxWordLength = 30;

        var wordLength = Math.min(word.length(), maxWordLength);
        var alphaSet = new HashSet<>();
        for(int i = 0; i < wordLength; ++i)   {
            char c = word.charAt(i);

            // if Hashset's add method return false, that means it is already present in HashSet
            if(!alphaSet.add(c))
                return false;
        }
        return true;
    }
}
