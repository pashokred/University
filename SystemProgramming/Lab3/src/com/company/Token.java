package com.company;

public class Token {
    private String tokenAttribute;
    private TokenType tokenType;

    public Token(String tokenAttribute, TokenType tokenType) {
        this.tokenAttribute = tokenAttribute;
        this.tokenType = tokenType;
    }

    public String toString() {
        return "<" + tokenAttribute + "> - <" + tokenType + ">";
    }
}

