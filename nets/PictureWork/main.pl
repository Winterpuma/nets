:- use_module(library(dif)).	% Sound inequality
:- use_module(library(clpfd)).	% Finite domain constraints
:- use_module(library(clpb)).	% Boolean constraints
:- use_module(library(chr)).	% Constraint Handling Rules
:- use_module(library(when)).	% Coroutining
%:- use_module(library(clpq)).  % Constraints over rational numbers
:- use_module(lambda).
:- use_module(library(apply)).
:- dynamic taken/2.

% [(Yi, [Xi])]
kek((X,Y),(X2,Y2)):-
        % поле на котором ищем 4 по У, 5 по Х
        Lst =[(0,[0,1,2,3,4]),
              (1,[0,1,2,3,4]),
              (2,[0,1,2,3,4]),
              (3,[0,1,2,3,4])],
        Fig1=(X,Y,[(0,[0]),
                    (1,[-1,0,1]),
                    (2,[0]) ] ),
        Fig2=(X2,Y2,[(0,[0,1,2]),
                    (1,[2]),
                    (2,[0,1,2])]),
        % надо осуществить подстановку и подобрать варанты размещения 
        % Fig1 и Fig2 в Lst
        place_it([Fig1,Fig2],Lst).
test1((X,Y),(X2,Y2)):-
	Lst = [	(0,[0,1,2,3,4]),
		(1,[0,1,2,3,4]),
		(2,[0,1,2,3,4])],
	Fig1 = (X,Y,[	(1,[2,1,0]),
			(2,[1]),
			(0,[1])]),
	Fig2 = (X2,Y2,[	(2,[2,1,0]),
			(0,[2,1,0]),
			(1,[1])]),
	place_it([Fig1,Fig2],Lst).

test2((X,Y),(X2,Y2)):-
	Lst = [	(0,[0,1,2,3,4]),
		(1,[0,1,2,3,4]),
		(2,[0,1,2,3,4])],
	Fig1 = (X,Y,[(0,[1]),
			(1,[0,1,2]),
			(2,[1])]),
	Fig2 = (X2,Y2,[	(0,[0,1,2]),
			(1,[1]),
			(2,[0,1,2])]),
	place_it([Fig1,Fig2],Lst).

test3(Ans):-
	Lst = [	(0,[0,1,2,3,4]),
		(1,[0,1,2,3,4]),
		(2,[0,1,2,3,4])],
	Fig1 = [(zero,[(0,[1]),
			(1,[0,1,2]),
			(2,[1])]),
			(pi,[(0,[1]),
			(1,[0,1,2]),
			(2,[1])] )],
	Fig2 = [(zero,[(0,[0,1,2]),
			(1,[1]),
			(2,[0,1,2])]),
			(pi,[(0,[0,1,2]),
			(1,[1]),
			(2,[0,1,2])])],
	place_it3([Fig1,Fig2],Lst,Ans).


test4(Ans):-
	Lst = [	(0,[0,1,2,3,4]),
		(1,[0,1,2,3,4]),
		(2,[0,1,2,3,4])],
	Fig1 = [(0,[1]),
			(1,[0,1,2]),
			(2,[1])],
	Fig2 = [(0,[0,1,2]),
			(1,[1]),
			(2,[0,1,2])],
	place_it2([Fig1,Fig2],Lst,Ans).

%place_it_rotate([[(Name,X,Y,Angle,Lst)|L_angles]|LFigs],Field
place_it2([],_,[]):-true.
place_it2([H|L],F,[(X,Y)|Ans]):- mymember(X,Y,F), delete_it((X,Y,H),F,F2),place_it2(L,F2,Ans).

place_it3(_,[],_):-fail.
place_it3([],_,[]):-true.
place_it3([[(Angle,H)|_]|L],F,[(X,Y,Angle)|Ans]):- mymember(X,Y,F), delete_it((X,Y,H),F,F2),place_it3(L,F2,Ans).
place_it3([[_|Hs]|L],F,Ans):- place_it3([Hs|L],F,Ans).

place_it([],_):-true.
place_it([(X,Y,Lst)|L],Field):-mymember(X,Y,Field),
                            delete_it((X,Y,Lst),Field,F2),
                            place_it(L,F2).
place_it([],X,X):-true.
place_it([(X,Y,Lst)|L],Field,Ans):-mymember(X,Y,Field),
                            delete_it((X,Y,Lst),Field,F2),
                            place_it(L,F2,Ans).
%mymember(_,_,[]):-fail.
mymember(X,Y,[(Y,Xs)|_]):- member(X, Xs).
mymember(X,Y,[_|L]):- mymember(X,Y, L).

delete_it1((_,_,_),[],_):-false.
delete_it1((X,Y,(Y1,X1s)),[(Y2,X2s)|F2],[(Y2,X3s)|F2]):-
   Y2 #= Y+Y1,
   mymap2(X2s,X1s,X,X1s2),
   subtract(X2s, X1s2, X3s).
delete_it1((X,Y,(Y1,X1s)),[Z|F],[Z|F2]):-
   delete_it1((X,Y,(Y1,X1s)),F,F2).

delete_it0((_,_,[]),F,F).
delete_it0((X,Y,[(Y1,X1s)|Lst]),Field,F2):-
    delete_it1((X,Y,(Y1,X1s)),Field,F1),
    delete_it0((X,Y,Lst),F1,F2).

delete_it((_,_,[]),F,F):-!.
delete_it((X,Y,[(Y1,X1s)|Lst]),[(Y2,X2s)|F],[(Y2,X3s)|F2]):-
    Y2 #= Y+Y1,
    %mymap(X1s,X,X1s2),
    %subset(X1s2,X2s),
    mymap2(X2s,X1s,X,X1s2),
    subtract(X2s, X1s2, X3s),
    delete_it((X,Y,Lst),F,F2).
delete_it((X,Y,Lst),[XYZ|F],[XYZ|F2]):-
    delete_it((X,Y,Lst),F,F2).
mymap([],_,[]):-!.
mymap([H|L],F,[H2|L2]):- H2 #= H+F, mymap(L,F,L2).
mymap2(_,[],_,[]):-!.
mymap2(Lst,[H|L],F,[H2|L2]):- H2 #= H+F, member(H2,Lst),!, mymap2(Lst,L,F,L2).


assert2(X):-assert(X).
assert2(X):-retract(X),fail.
retract2(X):-retract(X).
retract2(X):-asserta(X),fail.

isfree(X,Y):-between(0,5,X)
            ,between(0,2,Y)
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
tt(X,Y,Lst):- between(0,5,X),between(0,2,Y)
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