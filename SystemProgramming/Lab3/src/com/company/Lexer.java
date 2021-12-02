package com.company;

import java.io.File;
import java.io.FileNotFoundException;
import java.util.ArrayList;
import java.util.Scanner;
import java.util.regex.Matcher;
import java.util.regex.Pattern;

public class Lexer {
    private final static String preprocessorDirective = "^'use strict';";
    private final static String identifier = "^([a-zA-Z_$])([a-zA-Z_$0-9])*$";
    private final static String number = "\\b\\d+|\\b\\d+.\\d+|\\b\\d+e\\d+|0[xX][0-9a-fA-F]+";
    private final static String punctuator = "(\\(|\\)|\\[|]|\\{|}|,|:|;|\\.)";
    private final static String operator = "(\\+|-|\\*|/|=|%|\\+\\+|--|==|!=|>|<|>=|<=|&|\\||^"
            + "|~|<<|>>|>>>|&&|\\|\\||!|\\+=|-=|\\*=|/=|%=)";
    private final static String reserved = "abstract|boolean|break|byte|case|catch|char|class|continue"
            + "|const|debugger|default|delete|do|double|else|enum|export|extends|false|final|finally|float"
            + "|for|function|goto|if|implements|import|in|instanceof|int|interface|long|native|new|null|package"
            + "|private|protected|public|return|short|static|super|switch|synchronized|this|throw|throws|transient"
            + "|true|try|typeof|var|void|while|with";
    private final static String literals = "true|false|null|undefined";
    private final ArrayList<Token> tokens = new ArrayList<>();;
    private boolean inComment = false;

    public Lexer(File file) throws FileNotFoundException {
        Scanner scanner = new Scanner(file);
        while (scanner.hasNextLine()) {
            analyzeLine(scanner.nextLine());
        }
        scanner.close();
    }

    public String toString() {
        StringBuilder builder = new StringBuilder();
        for (Token token : tokens) {
            builder.append(token.toString());
            builder.append("\n");
        }
        return builder.toString();
    }

    private int matchString(String pattern, String string, int startSearchIndex) {
        Pattern p = Pattern.compile(pattern);
        Matcher m = p.matcher(string);
        if (m.find(startSearchIndex)) {
            return m.end();
        } else
            return -1;
    }

    private void analyzeLine(String line) {
        int i = 0;
        int index;

        while (i < line.length()) {
            // Remove spaces on start
            while (line.substring(i, i + 1).matches("\\s")) {
                if (i == line.length() - 1) {
                    break;
                }
                i++;
            }

            stopLoop isComment = checkForComment(i, line);
            i = isComment.i;
            if (isComment.stop)
                break;

            // Preprocessor directives detect
            if (line.matches(preprocessorDirective)) {
                tokens.add(new Token(line.substring(0, line.length()), TokenType.PREPROCESSOR_DIRECTIVE));
                break;
            }

            // String literal match
            stopLoop endOfString = matchString(i, line);
            i = endOfString.i;
            if (endOfString.stop)
                break;

            // Punctuator detect
            if (line.substring(i, i + 1).matches(punctuator)) {
                tokens.add(new Token(line.substring(i, i + 1), TokenType.PUNCTUATOR));
                i++;
                continue;
            }

            // Operators detect
            if (line.substring(i, i + 1).matches(operator)) {
                String op = line.substring(i, i + 1);
                if ((i < line.length() - 1) && line.substring(i, i + 2).matches(operator)) {
                    op = line.substring(i, i + 2);
                    if ((i < line.length() - 2) && line.substring(i, i + 3).matches(operator)) {
                        op = line.substring(i, i + 3);
                    }
                }
                i += op.length() - 1;
                tokens.add(new Token(op, TokenType.OPERATOR));
                i++;
                continue;
            }

            index = i;
            // Detect tokens in substrings
            if (line.substring(i, i + 1).matches("[0-9]")) {
                while (i < line.length() && !line.substring(i, i + 1).matches("\\s")
                        && (!line.substring(i, i + 1).matches(punctuator) || line.substring(i, i + 1).matches("\\."))
                        && !line.substring(i, i + 1).matches(operator)) {
                    i++;
                }
            } else {
                while (i < line.length() && !line.substring(i, i + 1).matches("\\s")
                        && !line.substring(i, i + 1).matches(punctuator)
                        && !line.substring(i, i + 1).matches(operator)) {
                    i++;
                }
            }
            String subString = line.substring(index, i);
            addToken(subString);

        }

    }

    private void addToken(String current) {
        if (current.matches(number))
            tokens.add(new Token(current, TokenType.NUMBER));
        else if (current.matches(preprocessorDirective))
            tokens.add(new Token(current, TokenType.PREPROCESSOR_DIRECTIVE));
        else if (current.matches(reserved))
            tokens.add(new Token(current, TokenType.RESERVED_WORD));
        else if (current.matches(literals))
            tokens.add(new Token(current, TokenType.LITERALS));
        else if (current.matches(identifier))
            tokens.add(new Token(current, TokenType.IDENTIFIER));
        else
            tokens.add(new Token(current, TokenType.ERROR));
    }

    private stopLoop checkForComment(Integer i, String line) {
        // Check for multiline comment
        int currentLineIndex = i;
        // Opening
        if (inComment) {
            int indexMatch = matchString("\\*/", line, currentLineIndex);
            if (indexMatch != -1) {
                i = indexMatch;
                inComment = false;
                if (indexMatch == line.length()) {
                    return new stopLoop(true, i);
                }
            } else
                return new stopLoop(true, i);

        } else
            // Closing comment
            if (i < line.length() - 1 && line.substring(i, i + 2).matches("/\\*")) {
                int indexMatch = matchString("\\*/", line, currentLineIndex);
                if (indexMatch != -1) {
                    i = indexMatch - 1;
                } else {
                    inComment = true;
                    return new stopLoop(true, i);
                }
            }

        // Check for inline comment
        if (i < line.length() - 1 && line.substring(i, i + 2).matches("//")) {
            return new stopLoop(true, i);
        }

        return new stopLoop(false, i);
    }

    private stopLoop matchString(Integer i, String line) {
        if (line.substring(i, i + 1).matches("\"")) {
            int matchedIndex = matchString("\"", line, i + 1);
            if (matchedIndex != -1) {
                tokens.add(new Token(line.substring(i, matchedIndex), TokenType.STRING_CONSTANT));

                i = matchedIndex;
            } else {
                tokens.add(new Token(line.substring(i), TokenType.ERROR));
                return new stopLoop(true, i);
            }
        }
        if (line.substring(i, i + 1).matches("'")) {
            int matchedIndex = matchString("'", line, i + 1);
            if (matchedIndex != -1) {
                tokens.add(new Token(line.substring(i, matchedIndex), TokenType.STRING_CONSTANT));

                i = matchedIndex;
            } else {
                tokens.add(new Token(line.substring(i), TokenType.ERROR));
                return new stopLoop(true, i);
            }
        }
        return new stopLoop(false, i);

    }

    private class stopLoop {

        public boolean stop;
        public int i;

        public stopLoop(boolean stop, int i) {
            this.i = i;
            this.stop = stop;
        }

    }
}
