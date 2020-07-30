:- use_module(library(dif)).	% Sound inequality
:- use_module(library(clpfd)).	% Finite domain constraints
:- use_module(library(clpb)).	% Boolean constraints
:- use_module(library(chr)).	% Constraint Handling Rules
:- use_module(library(when)).	% Coroutining
%:- use_module(library(clpq)).  % Constraints over rational numbers
:- use_module(lambda).
:- use_module(library(apply)).

% попробуем вставить в пустую полоску занятую полоску
%ins(_,_,[]).
ins((A,B),(A,A),[(A2,B)]):-A2 #= A+1,!.
ins((B,A),(A,A),[(B,A2)]):-A2 #= A-1,!.
ins((X_free_start,X_free_end),(X_s,X_e),Ans):-
  X_e >= X_s,
  between(X_free_start, X_free_end, X_s),
  between(X_free_start, X_free_end, X_e),
  Xs #= X_s - 1,
  Xe #= X_e + 1,
  Ans = [(X_free_start,Xs),(Xe,X_free_end)].

% в уже разбитую полоску вставить ещё одну полоску
ins2([H|L],Otr,A):- ins(H,Otr,H2),!, append(H2, L, A).
ins2([H|L],Otr,[H|A]):- ins2(L,Otr,A).

% в уже разбитую полоску вставить несколько полосок
ins3(L,[],L):-!.
ins3(L,[H|Otr],A):- ins2(L,H,A2), ins3(A2,Otr,A).

% без поворотов
place_it([],_):-true.
place_it([(X,Y,Lst)|L],Field):-
    H#=10, W#=10,
    between(0,H,Y), between(0,W,X),
    delete0(Field,Lst,(X,Y),Field2),
    place_it(L,Field2).
%
place_it0([],_,[]):-true.
place_it0([Lst|L],Field,[(X,Y)|Ans]):-
    H#=10, W#=10,
    between(0,H,Y), between(0,W,X),
    delete0(Field,Lst,(X,Y),Field2),
    place_it0(L,Field2,Ans).
% generate(10,10,F),place_it0(Fig,F,Ans).
% с поворотами
place_it3([],_,[]):-true.
place_it3(_,[],_):-fail.
place_it3([  [(Angle,H)|_]  |L],F,[(X,Y,Angle)|Ans]):-
    fromField((X,Y)),
    delete0(F,H,(X,Y),F2),
    place_it3(L,F2,Ans).
place_it3([[_|Hs]|L],F,Ans):- place_it3([Hs|L],F,Ans). % если  поворот не подошёл, пробуем другой градус

fromField((X,Y)):- H1#=299, W1#=99, between(0,H1,Y), between(0,W1,X).

test00(Ans) :- generate(299,99,F),
    Fig0 = [(0,[(-1,[(0,0)])] )],
    Fig1 = [(0,[(-1,[(0,0)]),(0,[(-1,1)]),(1,[(0,0)])])],
    place_it3([Fig0, Fig1],F,Ans).

% W -- ширина пространства, H -- высота, считать с 0, из W и H внутри 1 не вычитается
test0(W,H,Ans):-
    generate(W,H,Field),
    Fig=((X,Y),[(-1,[(0,1)]),
                (0,[(-1,2)]),
                (1,[(-1,-1),(2,2)])]),
    (_,Lst)=Fig,
    between(0,H,Y), between(0,W,X),
    delete0(Field,Lst,(X,Y),Field2),
    Ans=(X,Y).

% задавать пространство
generate(W,H,F):-findall((I,Lst),
                    (H1 #= H-0, W1 #= W-0,
                    between(0,H1,I), Lst=[(0,W1)]), F).
% удалятор из чего, кого, что_получается
delete0([],[],_, [] ):-!.
delete0(L,[],_, L ):-!.
delete0([],L,_, _ ):-!,fail.
delete0([(Y0,Lst0)|F],[(Yi,Lst)|L],(X,Y), [(Y0,Lst3)|F2] ):-
        Y0 #= Yi + Y,
        %convlist([(X1_1,X1_2),(X2_1,X2_2)]>>(X2_1 #= X1_1+X,
        %                                     X2_2 #= X1_2+X),Lst,Lst2),
        conv2(X,Lst,Lst2),
        ins3(Lst0,Lst2,Lst3),!,
        delete0(F,L,(X,Y),F2).
delete0([L0|F],L,(X,Y), [L0|F2] ):-
        delete0(F,L,(X,Y),F2).

conv2(_,[],[]):-!.
conv2(Delta,[(X,X1)|Lst1],[(X_,X1_)|LstAns]):-
	X_ is Delta+X,
	X1_ is Delta+X1,
	conv2(Delta,Lst1,LstAns).
