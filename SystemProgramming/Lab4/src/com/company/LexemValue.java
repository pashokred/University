package com.company;

public class LexemValue {
    private Lexems lex;
    private String val;

    public LexemValue(Lexems l, String v) {
        lex = l;
        val = v;
    }

    public String toString() {
        return "<" + lex + ">" + " - " + val + "\n";
    }
}