:- use_module(library(dif)).	% Sound inequality
:- use_module(library(clpfd)).	% Finite domain constraints
%:- use_module(library(clpb)).	% Boolean constraints
:- use_module(library(chr)).	% Constraint Handling Rules
:- use_module(library(when)).	% Coroutining
%:- use_module(library(clpq)).  % Constraints over rational numbers
:- use_module(lambda).
:- use_module(library(apply)).
:- use_module(library(thread)).


:- use_module(library(http/thread_httpd)).
:- use_module(library(http/http_dispatch)).
:- use_module(library(http/http_error)).
:- use_module(library(http/http_json)).
:- use_module(library(http/http_server)).
:- use_module(library(http/thread_httpd)).
:- use_module(library(http/http_header)).
:- use_module(library(http/http_multipart_plugin)).
:- use_module(library(http/http_client)).
:- use_module(library(http/html_write)).
:- use_module(library(option)).

%:- consult(figInfo).
%:- consult(queryFile).

server() :-
	http_server(http_dispatch, [port(8080)]).

queen(_R):-
	%make,
        consult([figInfo,queryFile]),
	myQuery(Ans),
	reply_json(json{answer:Ans}).

queen(_R):-
	format('Content-type: text/plain~n~nNoAnswer', []).

:- initialization server.
:- http_handler(/, queen, [method(M),methods([get,post]),time_limit(6000)]).



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

canIns3(_,[]):-!.
canIns3(L,Otr):- findall(Oi,(member(Oi,Otr),ins2(L,Oi,_)),Lst),
                length(Lst,LL),
                length(Otr,LO),
                 LL=LO.
                %concurrent_maplist(\Z^Z2^(ins2(L,Z,_),!,Z2=1;Z2=0), Otr, LL1),
                %sum_list(LL1, LL),

                %length(Otr,LO),
                % LL=LO.

% в уже разбитую полоску вставить несколько полосок
ins3(L,[],L):-!.
ins3(L,Otr,A):-canIns3(L,Otr),!, ins3_helper(L,Otr,A).
ins3_helper(L,[],L):-!.
%ins3([],L,_):-!,fail.
%ins3(L,[H|Otr],A):-ins2(L,H,A2), ins3(A2,Otr,A).
ins3_helper(L,[H|Otr],A):-ins2(L,H,A2), ins3_helper(A2,Otr,A).

% задавать пространство
generate(W,H,F):-findall((I,Lst),
                    (H1 #= H-0, W1 #= W-0,
                    between(0,H1,I), Lst=[(0,W1)]), F).
% удалятор из чего, кого, что_получается
delete0([],[],_, [] ):-!.%write('1'),!.
delete0(L,[],_, L ):-!.%write('2'),!.
delete0([],_,_, _ ):-!,fail. % write('3'),!,fail.
delete0([(Y0,Lst0)|F],[(Yi,Lst)|L],(X,Y), [(Y0,Lst3)|F2] ):- %write('d03'),nl,
        Y0 #= Yi + Y, %write(('ys',Y0,Yi,Y)),nl,
        %conv2(X,Lst,Lst2), %write(('d0',Lst2)),nl,
        %concurrent_maplist(\Z^Z2^(Z=(X_s,X_e),X_s2 is X_s + X, X_e2 is X_e + X, Z2=(X_s2,X_e2)), Lst, Lst2),
        maplist(\Z^Z2^(Z=(X_s,X_e),X_s2 is X_s + X, X_e2 is X_e + X, Z2=(X_s2,X_e2)), Lst, Lst2),
        ins3(Lst0,Lst2,Lst3),!,%write('222'),nl,
        delete0(F,L,(X,Y),F2).
delete0([L0|F],L,(X,Y), [L0|F2] ):-%write('d04'),nl, write((F,'asd',L)),nl,
        delete0(F,L,(X,Y),F2).

conv2(_,[],[]):-!.
conv2(Delta,[(X,X1)|Lst1],[(X_,X1_)|LstAns]):-
	X_ is Delta+X,
	X1_ is Delta+X1,
	conv2(Delta,Lst1,LstAns).

place_it3_2([],A,[],A):-true.
place_it3_2(_,[],_,_):-fail.
place_it3_2([  [(Angle,Delta,H)|_]  |L],F,[(X,Y,Angle)|Ans],QRes):-
    fromField2((X,Y),Delta, F),
    delete0(F,H,(X,Y),F2),
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
    canIns3(Test,D2).
inXs2([_|L],X,D,T):-inXs2(L,X,D,T).
dist((A,B),R):-R is B-A.




place_it3_3_helper([],A,[],A):-true.
place_it3_3_helper(_,[],_,_):-fail.
place_it3_3_helper([(_,_,_,[] )|_],
                     _,
                     _,
                     _):-fail.
% (MinY,MaxY) -- мин и макс Y-полосок у фигур
place_it3_3_helper([(Dx,Dy,(AngleMin,AngleMax),[(Angle,Delta,(MinY,MaxY),H)|_] )|L],
                     F,
                     [json{xCenter:X, yCenter:Y, angle: Angle}|Ans],
                     QRes):-
    between(AngleMin,AngleMax,Angle),
    %write('0'),
    %fromField2_helper((X,Y),Delta, F,Dx,Dy),
    fromField3_helper(Lst3, Delta,H, F,Dx,Dy,(MinY,MaxY)),
    %fromField2_helper2(Lst3, Delta,H, F,Dx,Dy),
    %write('1'),nl,
    member((Y,X),Lst3),%write('2'),
    delete0(F,H,(X,Y),F2),
    place_it3_3_helper(L,F2,Ans,QRes).
place_it3_3_helper([(Dx,Dy,Da,[_|Hs])|L],F,Ans,QRes):- place_it3_3_helper([(Dx,Dy,Da,Hs)|L],F,Ans,QRes).

%fromField2_helper((X,Y),Delta, F,(Xmin,Xmax),(Ymin,Ymax)):-
% (MinY,MaxY) -- мин и макс Y-полоски у фигуры
fromField3_helper(Lst3,Delta,H, F,(Xmin,Xmax),(Ymin,Ymax),(MinY,MaxY)):-
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
getFigApprocCoords(MagicNum, Kscale, Fig, FigPos, ScaledAns) :-
	MinX is floor(FigPos.xCenter * Kscale) - MagicNum,
	MaxX is ceiling(FigPos.xCenter * Kscale) + MagicNum,
	MinY is floor(FigPos.yCenter * Kscale) - MagicNum,
	MaxY is ceiling(FigPos.yCenter * Kscale) + MagicNum,
	MinAng is FigPos.angle - 3,
	MaxAng is FigPos.angle + 3,
	ScaledAns = ((MinX, MaxX), (MinY, MaxY), (MinAng, MaxAng), Fig).


getNextApprocCoords(_, [], [], []).
getNextApprocCoords(Kscale, [CurFig|TailFigs], [CurFigAns|TailAns], [CurApproc|TailApproc]) :-
	MagicNum is 30,
	getFigApprocCoords(MagicNum, Kscale, CurFig, CurFigAns, CurApproc),
	getNextApprocCoords(Kscale, TailFigs, TailAns, TailApproc).


% Загрузка файла через post, используя multipart
:- http_handler(root(upload/Filename), upload(Filename), []).

upload(Filename, Request) :-
	multipart_post_request(Request), !,
	http_read_data(Request, Parts,
		       [ on_filename(save_file)
		       ]),
	memberchk(file=file(OrigFileName, Saved), Parts),
	mv(Saved, Filename),
	format('Content-type: text/plain~n~n'),
	format('Saved your file "~w" into "~w"~n', [OrigFileName, Filename]).
upload(_Request) :-
	throw(http_reply(bad_request(bad_file_upload))).

multipart_post_request(Request) :-
	memberchk(method(post), Request),
	memberchk(content_type(ContentType), Request),
	http_parse_header_value(
	    content_type, ContentType,
	    media(multipart/'form-data', _)).

:- public save_file/3.

save_file(In, file(FileName, File), Options) :-
	option(filename(FileName), Options),
	setup_call_cleanup(
	    tmp_file_stream(octet, File, Out),
	    copy_stream_data(In, Out),
	    close(Out)).

:- multifile prolog:message//1.
