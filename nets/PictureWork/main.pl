:- use_module(library(dif)).	% Sound inequality
:- use_module(library(clpfd)).	% Finite domain constraints
:- use_module(library(clpb)).	% Boolean constraints
:- use_module(library(chr)).	% Constraint Handling Rules
:- use_module(library(when)).	% Coroutining
%:- use_module(library(clpq)).  % Constraints over rational numbers
:- use_module(lambda).
:- use_module(library(apply)).
:- dynamic taken/2.

assert2(X):-assert(X).
assert2(X):-retract(X),fail.
retract2(X):-retract(X).
retract2(X):-asserta(X),fail.

isfree(X,Y):-between(0,5,X)
            ,between(0,3,Y)
            , not(taken(X,Y)).
taken(-1,-1).
%taken(X,Y).

lol(X):-X>2.
%fig1(X,Y):-map(\X1^Y1^isfree(X+X1,Y+Y1),[])
%fig1(X,Y):-[(1,0),(0,0),(0,-1),(-1,0),(0,1)]
testfig(X,Y,Lst):-maplist(\Z^((X1,Y1)=Z,
                               X_1 #= X1 + X,
                               Y_1 #= Y1 + Y,
                               %write(X_1),write(';'),write(Y_1),nl,
                               isfree(X_1,Y_1)),Lst).
takefig(X,Y,Lst):-maplist(\Z^((X1,Y1)=Z,
                               X_1 #= X1 + X,
                               Y_1 #= Y1 + Y,
                               assert2(taken(X_1,Y_1))),Lst).

%tryPut([],A,A):-!.
%tryPut([El|Lst],LstAns,Res):-
% эта штука определяет, может ли заданная вещь поместиться на листе (одном)
tt(X,Y,Lst):- between(0,5,X),between(0,3,Y)
    %,write(X),write(';'),write(Y),
    ,testfig(X,Y,Lst),
    takefig(X,Y,Lst). %[(1,0),(0,0),(0,-1),(-1,0),(0,1)]).
    %,labeling([],[X,Y]).
% эта вещь вырабатывает подстановки для размещения всех элементов
% списка на одном листе
% должен ли это быть единственный способ?
f1([],A,A).
f1([(Name,Lst)|T],Acc,[(Name,X,Y)|Ans]):-tt(X,Y,Lst),f1(T,Acc,Ans).
%f1(Lst,Acc,Ans):-.
clear():-retractall(taken(_,_)).

% надо проверить, поместится ли этот набор с учётом поворотов 360
tryer360([],[]).%:-write('lol0').
tryer360([[(Name360,Lst)|_]|T],[(Name360,X,Y)|Ans]):-
     %write(Name360),nl,write(T),nl,
     tt(X,Y,Lst),%write('lll'),
     %write((X,Y)),nl,
     tryer360(T,Ans).
tryer360([[_|T360]|T],Ans):-tryer360([T360|T],Ans).
tryer360([[]|_],_):-false.

% надо проверить, поместится ли всё в одно разбиение
% если нет -- в 2,
% если нет -- в 3
% и т.д.
%mT(Lst,Acc,Ans):-.








f2([],A,B,A,B).
f2([H|T],Ac1,Ac2,Res1,Res2):-f2(T,[H|Ac1],Ac2,Res1,Res2).
f2([H|T],Ac1,Ac2,Res1,Res2):-f2(T,Ac1,[H|Ac2],Res1,Res2).
f_2([],[],[]).
f_2([H|T],[H|A],B):-f_2(T,A,B).
f_2([H|T],A,[H|B]):-f_2(T,A,B).

f3([],A,B,C,A,B,C).
f3([H|T],A,B,C,A1,B1,C1):-f3(T,[H|A],B,C,A1,B1,C1).
f3([H|T],A,B,C,A1,B1,C1):-f3(T,A,[H|B],C,A1,B1,C1).
f3([H|T],A,B,C,A1,B1,C1):-f3(T,A,B,[H|C],A1,B1,C1).

% тут список комбинаций id на 5 листов
f_5([],[],[],[],[],[]).
f_5([H|T],[H|L1],L2,L3,L4,L5):-f_5(T,L1,L2,L3,L4,L5).
f_5([H|T],L1,[H|L2],L3,L4,L5):-f_5(T,L1,L2,L3,L4,L5).
f_5([H|T],L1,L2,[H|L3],L4,L5):-f_5(T,L1,L2,L3,L4,L5).
f_5([H|T],L1,L2,L3,[H|L4],L5):-f_5(T,L1,L2,L3,L4,L5).
f_5([H|T],L1,L2,L3,L4,[H|L5]):-f_5(T,L1,L2,L3,L4,L5).

app(A,[],[[A]]) :- !.
app(A,[H|T],R) :- app(A,T,R1), append([[A|H]],R1,R).
 
bull([],[]) :- !.
bull([A|T],R) :- bull(T,R1), app(A,R1,R2), append(R1,R2,R).


% основная штука -- определяет минимально использованное количество листов
mainTest(Lst,R1,R2,R3,R4,R5):-f_5(Lst,L1,L2,L3,L4,L5),
                           % сделали разбиение на 5 листов
                           % нужно проверить, помещаются ли
                           f1(L1,[],R1),
                           f1(L2,[],R2),
                           f1(L3,[],R3),
                           f1(L4,[],R4),
                           f1(L5,[],R5)
                           .