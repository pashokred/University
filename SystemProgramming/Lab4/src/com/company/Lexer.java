package com.company;

import java.util.ArrayList;
import java.util.Arrays;
import java.util.List;

enum Lexems {
    RESERVED, NUMBER, STRING, OPERATOR, PUNCTATION, IDENTIFIER, ERROR, DIRECTIVE
}

public class Lexer {
    private List<String> reserved = Arrays.asList("abstract", "boolean", "break", "byte", "case", "catch", "char",
            "class", "continue ", "const", "debugger", "default", "delete", "do", "double", "else", "enum", "export",
            "extends", "false", "final", "finally", "float", "for", "function", "goto", "if", "implements", "import",
            "in", "instanceof", "int", "interface", "long", "native", "new", "null", "package ", "private", "protected",
            "public", "return", "short", "static", "super", "switch", "synchronized", "this", "throw", "throws",
            "transient ", "true", "try", "typeof", "var", "void", "while", "with");

    private List<String> directive = Arrays.asList("'use strict';");

    private List<LexemValue> lexems = new ArrayList<LexemValue>();
    private boolean isComment = false;

    public void DisplayLexems() {
        for (LexemValue t : lexems) {
            System.out.println(t);
        }
    }

    public void startState(String l) {
        if (!isComment)
            LineState(l);
        else
            CommentState(l);
    }

    private void CommentState(String line) {
        for (int i = 0; i < line.length() - 1; i++) {
            if (line.charAt(i) == '*' && line.charAt(i + 1) == '/') {
                isComment = false;
                LineState(line.substring(i + 2));
                break;
            }
        }
    }

    private void QuoteState(String line) {
        StringBuilder word = new StringBuilder();
        word.append('\'');
        for (int i = 0; i < line.length(); i++) {
            if (line.charAt(i) == '\'') {
                word.append('\'');
                LexemValue lv = new LexemValue(Lexems.STRING, word.toString());
                lexems.add(lv);
                LineState(line.substring(i + 1));
                return;
            } else {
                word.append(line.charAt(i));
            }
        }
        LexemValue lv = new LexemValue(Lexems.ERROR, word.toString());
        lexems.add(lv);
    }

    private void DoubleQuoteState(String line) {
        StringBuilder word = new StringBuilder();
        word.append('\"');
        for (int i = 0; i < line.length(); i++) {
            if (line.charAt(i) == '\"') {
                word.append('\"');
                LexemValue lv = new LexemValue(Lexems.STRING, word.toString());
                lexems.add(lv);
                LineState(line.substring(i + 1));
                return;
            } else {
                word.append(line.charAt(i));
            }
        }
        LexemValue lv = new LexemValue(Lexems.ERROR, word.toString());
        lexems.add(lv);
    }

    private void NumberState(String line) {
        StringBuilder word = new StringBuilder();
        LexemValue lv;
        for (int i = 0; i < line.length(); i++) {
            if (Character.isDigit(line.charAt(i)) || Character.isAlphabetic(line.charAt(i)) || line.charAt(i) == '.') {
                word.append(line.charAt(i));
            } else {
                String w = word.toString();
                if (Check_Hex(w) || Check_Integer(w) || Check_Double(w))
                    lv = new LexemValue(Lexems.NUMBER, w);
                else
                    lv = new LexemValue(Lexems.ERROR, w);
                lexems.add(lv);
                LineState(line.substring(i));
                return;
            }
        }
        String w = word.toString();
        if (Check_Hex(w) || Check_Integer(w) || Check_Double(w))
            lv = new LexemValue(Lexems.NUMBER, w);
        else
            lv = new LexemValue(Lexems.ERROR, w);
        lexems.add(lv);
    }

    private void LineState(String l) {
        String line = l.trim();
        int length = line.length();
        for (int i = 0; i < length; i++) {
            char currCharacter = line.charAt(i);
            switch (currCharacter) {
                case '>': {
                    LexemValue lv;
                    if (i != length - 1 && line.charAt(i + 1) == '=') {
                        lv = new LexemValue(Lexems.OPERATOR, ">=");
                        i++;
                    } else {
                        lv = new LexemValue(Lexems.OPERATOR, ">");
                    }
                    lexems.add(lv);
                    break;
                }
                case '&': {
                    LexemValue lv;
                    if (i != length - 1 && line.charAt(i + 1) == '&') {
                        lv = new LexemValue(Lexems.OPERATOR, "&&");
                        i++;
                    } else {
                        lv = new LexemValue(Lexems.OPERATOR, "&");
                    }
                    lexems.add(lv);
                    break;
                }
                case '|': {
                    LexemValue lv;
                    if (i != length - 1 && line.charAt(i + 1) == '|') {
                        lv = new LexemValue(Lexems.OPERATOR, "||");
                        i++;
                    } else {
                        lv = new LexemValue(Lexems.OPERATOR, "|");
                    }
                    lexems.add(lv);
                    break;
                }
                case '<': {
                    LexemValue lv;
                    if (i != length - 1 && line.charAt(i + 1) == '=') {
                        lv = new LexemValue(Lexems.OPERATOR, "<=");
                        i++;
                    } else {
                        lv = new LexemValue(Lexems.OPERATOR, "<");
                    }
                    lexems.add(lv);
                    break;
                }
                case '=': {
                    LexemValue lv;
                    if (i != length - 1 && line.charAt(i + 1) == '=') {
                        lv = new LexemValue(Lexems.OPERATOR, "==");
                        i++;
                    } else {
                        lv = new LexemValue(Lexems.OPERATOR, "=");
                    }
                    lexems.add(lv);
                    break;
                }
                case '!': {
                    LexemValue lv;
                    if (i != length - 1 && line.charAt(i + 1) == '=') {
                        lv = new LexemValue(Lexems.OPERATOR, "!=");
                        i++;
                    } else {
                        lv = new LexemValue(Lexems.OPERATOR, "!");
                    }
                    lexems.add(lv);
                    break;
                }
                case '*': {
                    LexemValue lv;
                    if (i != length - 1 && line.charAt(i + 1) == '=') {
                        lv = new LexemValue(Lexems.OPERATOR, "*=");
                        i++;
                    } else {
                        lv = new LexemValue(Lexems.OPERATOR, "*");
                    }
                    lexems.add(lv);
                    break;
                }
                case '%': {
                    LexemValue lv;
                    if (i != length - 1 && line.charAt(i + 1) == '=') {
                        lv = new LexemValue(Lexems.OPERATOR, "%=");
                        i++;
                    } else {
                        lv = new LexemValue(Lexems.OPERATOR, "%");
                    }
                    lexems.add(lv);
                    break;
                }
                case '/': {
                    LexemValue lv;
                    if (i != length - 1 && line.charAt(i + 1) == '=') {
                        lv = new LexemValue(Lexems.OPERATOR, "/=");
                        lexems.add(lv);
                        i++;
                        break;

                    } else
                        // Comments zone
                        if (i != length - 1 && line.charAt(i + 1) == '/') {
                            return;
                        } else if (i != length - 1 && line.charAt(i + 1) == '*') {
                            isComment = true;
                            CommentState(line.substring(i));
                            return;
                        } else {
                            lv = new LexemValue(Lexems.OPERATOR, "/");
                            lexems.add(lv);
                            break;
                        }
                }
                case '+': {
                    LexemValue lv;
                    if (i != length - 1 && line.charAt(i + 1) == '=') {
                        lv = new LexemValue(Lexems.OPERATOR, "+=");
                        i++;
                    } else if (i != length - 1 && line.charAt(i + 1) == '+') {
                        lv = new LexemValue(Lexems.OPERATOR, "++");
                        i++;
                    } else {
                        lv = new LexemValue(Lexems.OPERATOR, "+");
                    }
                    lexems.add(lv);
                    break;
                }
                case '-': {
                    LexemValue lv;
                    if (i != length - 1 && line.charAt(i + 1) == '=') {
                        lv = new LexemValue(Lexems.OPERATOR, "-=");
                        i++;
                    } else if (i != length - 1 && line.charAt(i + 1) == '-') {
                        lv = new LexemValue(Lexems.OPERATOR, "--");
                        i++;
                    } else {
                        lv = new LexemValue(Lexems.OPERATOR, "-");
                    }
                    lexems.add(lv);
                    break;
                }
                case '(': {
                    LexemValue lv = new LexemValue(Lexems.PUNCTATION, "(");
                    lexems.add(lv);
                    break;
                }
                case ')': {
                    LexemValue lv = new LexemValue(Lexems.PUNCTATION, ")");
                    lexems.add(lv);
                    break;
                }
                case '{': {
                    LexemValue lv = new LexemValue(Lexems.PUNCTATION, "{");
                    lexems.add(lv);
                    break;
                }
                case '}': {
                    LexemValue lv = new LexemValue(Lexems.PUNCTATION, "}");
                    lexems.add(lv);
                    break;
                }
                case '[': {
                    LexemValue lv = new LexemValue(Lexems.PUNCTATION, "[");
                    lexems.add(lv);
                    break;
                }
                case ']': {
                    LexemValue lv = new LexemValue(Lexems.PUNCTATION, "]");
                    lexems.add(lv);
                    break;
                }
                case ',': {
                    LexemValue lv = new LexemValue(Lexems.PUNCTATION, ",");
                    lexems.add(lv);
                    break;
                }
                case '.': {
                    LexemValue lv = new LexemValue(Lexems.PUNCTATION, ".");
                    lexems.add(lv);
                    break;
                }
                case ';': {
                    LexemValue lv = new LexemValue(Lexems.OPERATOR, ";");
                    lexems.add(lv);
                    break;
                }
                case '\"': {
                    DoubleQuoteState(line.substring(i + 1));
                    return;
                }
                case '\'': {
                    QuoteState(line.substring(i + 1));
                    return;
                }
                case ' ': {
                    break;
                }
                default: {
                    if (Character.isDigit(currCharacter)) {
                        NumberState(line.substring(i));
                        return;
                    } else if (Character.isLetter(currCharacter) || currCharacter == '_' || currCharacter == '\'') {
                        WordState(line.substring(i));
                        return;
                    } else {
                        LexemValue lv = new LexemValue(Lexems.ERROR, Character.toString(currCharacter));
                        lexems.add(lv);
                        break;
                    }
                }
            }
        }
    }

    private void WordState(String line) {
        StringBuilder word = new StringBuilder();
        LexemValue lv;
        for (int i = 0; i < line.length(); i++) {
            if (Character.isAlphabetic(line.charAt(i)) || Character.isDigit(line.charAt(i)) || line.charAt(i) == '_'
                    || line.charAt(i) == '\'') {
                word.append(line.charAt(i));
            } else {
                int a = 0;
                String w = word.toString();
                if (reserved.contains(w))
                    lv = new LexemValue(Lexems.RESERVED, w);
                else if (directive.contains(w)) {
                    lv = new LexemValue(Lexems.DIRECTIVE, w);
                } else if (Check_Identifier(w))
                    lv = new LexemValue(Lexems.IDENTIFIER, w);
                else
                    lv = new LexemValue(Lexems.ERROR, w);
                lexems.add(lv);

                LineState(line.substring(i));
                return;
            }
        }
        String w = word.toString();
        if (reserved.contains(w))
            lv = new LexemValue(Lexems.RESERVED, w);
        else if (Check_Identifier(w))
            lv = new LexemValue(Lexems.IDENTIFIER, w);
        else
            lv = new LexemValue(Lexems.ERROR, w);
        lexems.add(lv);
    }

    private boolean Check_Integer(String strNum) {
        try {
            if (strNum == null) {
                return false;
            }
            int d = Integer.parseInt(strNum);
        } catch (NumberFormatException nfe) {
            return false;
        }
        return true;
    }

    private boolean Check_Double(String strNum) {
        try {
            if (strNum == null) {
                return false;
            }
            double d = Double.parseDouble(strNum);
        } catch (NumberFormatException nfe) {
            return false;
        }
        return true;
    }

    private boolean Check_Hex(String strHex) {
        if (strHex.length() > 2) {
            if (strHex.charAt(0) == '0' && (strHex.charAt(1) == 'x' || strHex.charAt(1) == 'X')) {
                for (int i = 2; i < strHex.length(); i++) {
                    if (!Character.isDigit(strHex.charAt(i)) && !(strHex.charAt(i) > 64 && strHex.charAt(i) < 71)
                            && !(strHex.charAt(i) > 96 && strHex.charAt(i) < 103))
                        return false;
                }
                return true;
            }
            return false;
        } else
            return false;
    }

    private boolean Check_Identifier(String strIdent) {
        char first = strIdent.charAt(0);
        if (first == '_' || Character.isAlphabetic(first)) {
            for (int i = 1; i < strIdent.length(); i++) {
                if (!Character.isAlphabetic(strIdent.charAt(i)) && !Character.isDigit(strIdent.charAt(i))
                        && strIdent.charAt(i) != '_')
                    return false;

            }
            return true;
        } else
            return false;
    }

}
