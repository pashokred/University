package com.company;

import java.util.*;

public class Automaton {
    private static class Transition {

        // code point
        public final int input;
        public int state;

        // aInput - code point
        Transition(int aInput, int aState) {
            input = aInput;
            state = aState;
        }

        public boolean compare(Transition objToCompare, Integer i1, Integer i2) {
            // Check for self link
            if ((input == objToCompare.input) && (state == i1) && (objToCompare.state == i2)) {
                return true;
            }
            return (input == objToCompare.input) && (state == objToCompare.state);

        }
    }

    private static class State {

        public final ArrayList<Transition> transitions = new ArrayList<>();

        // input - code point
        public void addTransition(int input, int nextStage) {
            transitions.add(new Transition(input, nextStage));
        }
    }

    private final int startState;
    private final int nAlphabet;
    private int statesCount;
    // [Transitions({input:letter, state: nextStage},{input:letter, state:
    // nextStage}),...]
    private final ArrayList<State> statesTo;

    // [Transitions({input:letter, state: curState},{input:letter, state:
    // curState}),...]
    private final ArrayList<State> statesFrom;
    private HashSet<Integer> finalStates = new HashSet<>();

    public boolean removeEqualStates() {
        ArrayList<Integer> equalGroup = findGroupOfEquality();

        if (equalGroup.size() > 0) {
            System.out.println("Equal groups" + equalGroup);
            Integer i1 = equalGroup.get(0);
            Integer i2 = equalGroup.get(1);
            Integer min = i1 > i2 ? i2 : i1;
            int max = i1 > i2 ? i1 : i2;

            statesTo.forEach(st -> st.transitions.forEach(tr -> {
                if (tr.state == max) {
                    tr.state = min;
                }
            }));

            removeState(max);
            return true;
        }
        return false;
    }

    public ArrayList<Integer> findGroupOfEquality() {
        ArrayList<Integer> indexes = new ArrayList<>();
        for (State s1 : statesTo) {
            if (indexes.size() > 0)
                break;
            for (State s2 : statesTo) {
                if (s1 == s2) {
                    continue;
                }

                if (compareTransitions(s1, s2)) {
                    indexes.add(statesTo.indexOf(s1));
                    indexes.add(statesTo.indexOf(s2));
                    break;
                }

            }
        }
        return indexes;
    }

    private boolean compareTransitions(State state1, State state2) {

        Integer State1Index = statesTo.indexOf(state1);
        Integer State2Index = statesTo.indexOf(state2);

        ArrayList<Transition> StateTransitions1 = state1.transitions;
        ArrayList<Transition> StateTransitions2 = state2.transitions;
        final ArrayList<Boolean> equality = new ArrayList<>(Collections.nCopies(StateTransitions1.size(), false));
        if (StateTransitions1.size() != StateTransitions2.size()) {
            return false;
        }

        StateTransitions1.forEach(transition1 -> StateTransitions2.forEach(transition2 -> {
            if (transition1.compare(transition2, State1Index, State2Index)) {
                equality.set(StateTransitions1.indexOf(transition1), true);
            }
        }));
        equality.removeIf(e -> e);
        return equality.size() == 0;
    }

    private String stringifyPaths() {
        ArrayList<String> finalStatesToList = new ArrayList<>();
        for (State i : statesTo) {
            for (Transition t : i.transitions) {
                finalStatesToList.add(statesTo.indexOf(i) + " " + (char) t.input + " " + t.state);
            }
        }
        return String.join("\n", finalStatesToList);
    }

    private String stringifyFinalStates() {
        ArrayList<String> finalStatesString = new ArrayList<>();
        for (Integer i : finalStates) {
            finalStatesString.add(" " + i);
        }
        return String.join("", finalStatesString);

    }

    @Override
    public String toString() {
        System.out.println(" \n Automaton:");

        return nAlphabet + "\n" + statesCount + "\n" + startState + "\n" + finalStates.size()
                + stringifyFinalStates() + "\n" + stringifyPaths();
    }

    public Automaton(int nState, int aStart, int nAlphabet) {
        startState = aStart;
        statesTo = new ArrayList<>(nState);
        statesFrom = new ArrayList<>(nState);
        this.nAlphabet = nAlphabet;
        for (int i = 0; i < nState; ++i) {
            statesTo.add(new State());
            statesFrom.add(new State());
        }
        statesCount = statesTo.size();
    }

    public void addTransition(int curState, int input, int nextStage) {
        statesTo.get(curState).addTransition(input, nextStage);
        statesFrom.get(nextStage).addTransition(input, curState);
    }

    public void addFinalState(int state) {
        finalStates.add(state);
    }

    private ArrayList<Boolean> formReachableList(List<State> states, int aStartState) {
        // [false,...false]
        final ArrayList<Boolean> visited = new ArrayList<>(Collections.nCopies(states.size(), false));
        final Queue<Integer> q = new LinkedList<>();
        q.offer(aStartState);

        while (!q.isEmpty()) {
            final int nextSt = q.remove();
            visited.set(nextSt, true);
            for (var t : states.get(nextSt).transitions) {
                if (!visited.get(t.state)) {
                    q.offer(t.state);
                }
            }
        }

        return visited;
    }

    public HashSet<Integer> deadStates() {
        final HashSet<Integer> result = new HashSet<>();
        final ArrayList<Boolean> undeadStates = new ArrayList<>(Collections.nCopies(statesFrom.size(), false));

        for (var finalSt : finalStates) {
            final var visited = formReachableList(statesFrom, finalSt);
            for (int i = 0; i < visited.size(); ++i) {
                if (visited.get(i)) {
                    undeadStates.set(i, true);
                }
            }
        }

        for (int i = 0; i < undeadStates.size(); ++i) {
            if (!undeadStates.get(i)) {
                result.add(i);
            }
        }
        return result;
    }

    public HashSet<Integer> unreachableStates() {
        final HashSet<Integer> result = new HashSet<>();
        final var visited = formReachableList(statesTo, startState);
        for (int i = 0; i < visited.size(); ++i) {
            if (!visited.get(i)) {
                result.add(i);
            }
        }
        return result;
    }

    public void removeUnreachableStates() {
        while (unreachableStates().size() != 0) {
            removeState(unreachableStates().iterator().next());
        }
    }

    public void removeDeadStates() {
        while (deadStates().size() > 0) {
            removeState(deadStates().iterator().next());
        }
    }

    private void removeState(int state) {
        statesTo.forEach(st -> st.transitions.forEach(tr -> {
            if (tr.state >= state) {
                tr.state--;
            }
        }));

        HashSet<Integer> newFinalSteps = new HashSet<>();
        finalStates.forEach(e -> {
            if (e == state) {
                return;
            }
            if (e > state) {
                newFinalSteps.add(e - 1);
                return;
            }
            newFinalSteps.add(e);
            return;
        });

        statesTo.remove(state);
        statesFrom.remove(state);
        finalStates = newFinalSteps;
        statesCount--;
    }

}
