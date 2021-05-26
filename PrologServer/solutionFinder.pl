:- use_module(library(dif)).	% Sound inequality
:- use_module(library(clpfd)).	% Finite domain constraints
%:- use_module(library(clpb)).	% Boolean constraints
:- use_module(library(chr)).	% Constraint Handling Rules
:- use_module(library(when)).	% Coroutining
%:- use_module(library(clpq)).  % Constraints over rational numbers
:- use_module(lambda).
:- use_module(library(apply)).
:- use_module(library(thread)).


% попробуем вставить в пустую полоску занятую полоску
%ins(куда,кого,[результат вставки]).
ins((Left1,_),(Left2,_),_):-Left1>Left2,!,fail.
ins((_,Right1),(_,Right2),_):-Right1<Right2,!,fail.
ins((Left1,Right1),(Left2,Right2),[]):-Left1=Left2,Right1=Right2,!.
ins((Left1,Right1),(Left2,Right2),[(Right2,Right1)]):-Left1=Left2,!.
ins((Left1,Right1),(Left2,Right2),[(Left1,Left2)]):-Right1=Right2,!.
ins((X_free_start,X_free_end),(X_s,X_e),Ans):-
  X_e >= X_s,
  X_free_start =< X_free_end,
  X_e < X_free_end, X_e>X_free_start,
  X_s < X_free_end, X_s>X_free_start,
  %between(X_free_start, X_free_end, X_s),
  %between(X_free_start, X_free_end, X_e),
  Xs is X_s - 1,
  Xe is X_e + 1,
  Ans = [(X_free_start,Xs),(Xe,X_free_end)].

% в уже разбитую полоску вставить ещё одну полоску
%ins2(L,Otr,A):-bagof(Ai, (member(Li,L),ins(Li,Otr,Ai)), As), flatten(As,A).

ins2([H|L],Otr,A):- ins(H,Otr,H2),!, append(H2, L, A).
ins2([H|L],Otr,[H|A]):- ins2(L,Otr,A).

can_instert_segments(_,[]):-!.
can_instert_segments(L,Otr):- findall(Oi,(member(Oi,Otr),ins2(L,Oi,_)),Lst),
                length(Lst,LL),
                length(Otr,LO),
                 LL=LO.
                %concurrent_maplist(\Z^Z2^(ins2(L,Z,_),!,Z2=1;Z2=0), Otr, LL1),
                %sum_list(LL1, LL),

                %length(Otr,LO),
                % LL=LO.

% в уже разбитую полоску вставить несколько полосок
instert_segments(L,[],L):-!.
instert_segments(L,Otr,A):-can_instert_segments(L,Otr),!, instert_segments_helper(L,Otr,A).
instert_segments_helper(L,[],L):-!.
%instert_segments([],L,_):-!,fail.
%instert_segments(L,[H|Otr],A):-ins2(L,H,A2), instert_segments(A2,Otr,A).
instert_segments_helper(L,[H|Otr],A):-ins2(L,H,A2), instert_segments_helper(A2,Otr,A).

% задавать пространство
generate(W,H,F):-findall((I,Lst),
                    (H1 #= H-0, W1 #= W-0,
                    between(0,H1,I), Lst=[(0,W1)]), F).
% удалятор из чего, кого, что_получается
delete_figure_from_lst([],[],_, [] ):-!.%write('1'),!.
delete_figure_from_lst(L,[],_, L ):-!.%write('2'),!.
delete_figure_from_lst([],_,_, _ ):-!,fail. % write('3'),!,fail.
delete_figure_from_lst([(Y0,Lst0)|F],[(Yi,Lst)|L],(X,Y), [(Y0,Lst3)|F2] ):- %write('d03'),nl,
        Y0 #= Yi + Y, %write(('ys',Y0,Yi,Y)),nl,
        %conv2(X,Lst,Lst2), %write(('d0',Lst2)),nl,
        %concurrent_maplist(\Z^Z2^(Z=(X_s,X_e),X_s2 is X_s + X, X_e2 is X_e + X, Z2=(X_s2,X_e2)), Lst, Lst2),
        maplist(\Z^Z2^(Z=(X_s,X_e),X_s2 is X_s + X, X_e2 is X_e + X, Z2=(X_s2,X_e2)), Lst, Lst2),
        instert_segments(Lst0,Lst2,Lst3),!,%write('222'),nl,
        delete_figure_from_lst(F,L,(X,Y),F2).
delete_figure_from_lst([L0|F],L,(X,Y), [L0|F2] ):-%write('d04'),nl, write((F,'asd',L)),nl,
        delete_figure_from_lst(F,L,(X,Y),F2).

conv2(_,[],[]):-!.
conv2(Delta,[(X,X1)|Lst1],[(X_,X1_)|LstAns]):-
	X_ is Delta+X,
	X1_ is Delta+X1,
	conv2(Delta,Lst1,LstAns).

place_it3_2([],A,[],A):-true.
place_it3_2(_,[],_,_):-fail.
place_it3_2([  [(Angle,Delta,H)|_]  |L],F,[(X,Y,Angle)|Ans],QRes):-
    fromField2((X,Y),Delta, F),
    delete_figure_from_lst(F,H,(X,Y),F2),
    place_it3_2(L,F2,Ans,QRes).
place_it3_2([[_|Hs]|L],F,Ans,QRes):- place_it3_2([Hs|L],F,Ans,QRes). % если  поворот не подошёл, пробуем другой градус





fromField2((X,Y),D,F):-findIn22(F,Y,X,D).

findIn22(F,Y,X,D):-
    %findall((Yi,Xs),(member((Yi,Xs),F)),Lst),
    %member((Y,Xs2),Lst), inXs2(Xs2,X,D,Xs2).
    findall((Yi,Xs), ( member((Yi,Xs),F)
                        )  ,Lst),
    %write(('0',Lst)),nl,
    maplist(\Z^Z2^(Z=(Yi,Xs)
                            ,findall((Yi,Xi),inXs2(Xs,Xi,D,Xs),Z2)),Lst,Lst2),
    %write(Lst2),nl,
    flatten(Lst2,Lst3),
    member((Y,X),Lst3). %, member(X,Xs_).

findIn2([(Y,Xs)|_],Y,X,D):-inXs2(Xs,X,D,Xs).
findIn2([_|L],Y,X,D):-findIn2(L,Y,X,D).

inXs2([(Xs,Xe)|_],X,D,Test):-[First|_]=D,
                            dist((Xs,Xe),Dist1),
                            dist(First,Dist2),
                            Dist1>=Dist2,
    between(Xs,Xe,X),conv2(X,D,D2),
    can_instert_segments(Test,D2).
inXs2([_|L],X,D,T):-inXs2(L,X,D,T).
dist((A,B),R):-R is B-A.




place_figures_in_range([],A,[],A):-true.
place_figures_in_range(_,[],_,_):-fail.
place_figures_in_range([(_,_,_,[] )|_],
                     _,
                     _,
                     _):-fail.
% (MinY,MaxY) -- мин и макс Y-полосок у фигур
place_figures_in_range([(Dx,Dy,(AngleMin,AngleMax),[(Angle,Delta,(MinY,MaxY),H)|_] )|L],
                     F,
                     [json{xCenter:X, yCenter:Y, angle: Angle}|Ans],
                     QRes):-
    between(AngleMin,AngleMax,Angle),
    %write('0'),
    %fromField2_helper((X,Y),Delta, F,Dx,Dy),
    match_pre_fit(Lst3, Delta,H, F,Dx,Dy,(MinY,MaxY)),
    %fromField2_helper2(Lst3, Delta,H, F,Dx,Dy),
    %write('1'),nl,
    member((Y,X),Lst3),%write('2'),
    delete_figure_from_lst(F,H,(X,Y),F2),
    place_figures_in_range(L,F2,Ans,QRes).
place_figures_in_range([(Dx,Dy,Da,[_|Hs])|L],F,Ans,QRes):- place_figures_in_range([(Dx,Dy,Da,Hs)|L],F,Ans,QRes).

%fromField2_helper((X,Y),Delta, F,(Xmin,Xmax),(Ymin,Ymax)):-
% (MinY,MaxY) -- мин и макс Y-полоски у фигуры
match_pre_fit(Lst3,Delta,H, F,(Xmin,Xmax),(Ymin,Ymax),(MinY,MaxY)):-
        %write('00'),nl,
        bagof((Yi,Xs0),
                (between(Ymin,Ymax,Yi),member((Yi,Xs0),F),
                Yi22 is Yi +MaxY, Yi23 is Yi+MinY,
                % что максимальные габариты помещаются
                member((Yi22,Xs022),F),member((Yi23,Xs023),F)
                % что в самых удалённых строках есть нужные зоны
                , not(Xs022=[]), not(Xs023=[])
                )
                ,Lst),
        %write('ys'),nl,
        % что помещается Delta
        concurrent_maplist(\Z^Z2^(Z=(Yi,Xs)
                            ,%finder1(Yi,Xs,Dx,Delta,Z2)
                            findall((Yi,Xi),
                            (between(Xmin,Xmax,Xi),inXs2(Xs,Xi,Delta,Xs))
                            ,Z2)
                            ),Lst,Lst2),
        %write('xs'),nl,
        flatten(Lst2,Lst3), not(Lst3=[]).

%fromField2_helper((X,Y),Delta, F,(Xmin,Xmax),(Ymin,Ymax)):-
fromField2_helper(Lst3,Delta, F,(Xmin,Xmax),(Ymin,Ymax)):-
        %write('00'),nl,
        bagof((Yi,Xs0),
                (between(Ymin,Ymax,Yi),member((Yi,Xs0),F))
                ,Lst),
        %write('ys'),nl,
        maplist(\Z^Z2^(Z=(Yi,Xs)
                            ,%finder1(Yi,Xs,Dx,Delta,Z2)
                            findall((Yi,Xi),
                            (between(Xmin,Xmax,Xi),inXs2(Xs,Xi,Delta,Xs))
                            ,Z2)
                            ),Lst,Lst2),
        %write('xs'),nl,
        flatten(Lst2,Lst3), not(Lst3=[]).

% Преобразует старые координаты в новые (для следующего масштаба)
get_fig_approc_coords(MagicNum, Kscale, Fig, FigPos, ScaledAns) :-
	MinX is floor(FigPos.xCenter * Kscale) - MagicNum,
	MaxX is ceiling(FigPos.xCenter * Kscale) + MagicNum,
	MinY is floor(FigPos.yCenter * Kscale) - MagicNum,
	MaxY is ceiling(FigPos.yCenter * Kscale) + MagicNum,
	MinAng is FigPos.angle - 3,
	MaxAng is FigPos.angle + 3,
	ScaledAns = ((MinX, MaxX), (MinY, MaxY), (MinAng, MaxAng), Fig).


get_next_approc_coords(_, [], [], []).
get_next_approc_coords(Kscale, [CurFig|TailFigs], [CurFigAns|TailAns], [CurApproc|TailApproc]) :-
	MagicNum is 30,
	get_fig_approc_coords(MagicNum, Kscale, CurFig, CurFigAns, CurApproc),
	get_next_approc_coords(Kscale, TailFigs, TailAns, TailApproc).
