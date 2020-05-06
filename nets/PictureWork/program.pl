
plane_img([[1, 1, 1, 1, 1, 1, 1], 
          [1, 0, 0, 0, 0, 0, 1],
          [1, 0, 0, 0, 0, 0, 1],
          [1, 0, 0, 0, 0, 0, 1],
          [1, 0, 0, 0, 0, 0, 1],
          [1, 0, 0, 0, 0, 0, 1],
          [1, 1, 1, 1, 1, 1, 1]]).

% это x
figure_one([[0, 0],
           [1, 1],
           [-1, -1],
           [1, -1],
           [-1, 1]]).
            
print_matr([], _):-!.
print_matr([Head|Tail], SepFunctor):-
    write(Head), call(SepFunctor),
  	print_matr(Tail, SepFunctor).

get_matrix_element(Matr, RowIndex, ColumnIndex, Element):-
    nth0(RowIndex, Matr, Row),
	nth0(ColumnIndex, Row, Element).

% repl_list(OldList, ColNum, NewEl, ResList)
% will replace element in colNum position to NewEl
repl_list([], _Col, _El, []).

repl_list([_|Tail], 0, El, [El|TailResult]):-
   	repl_list(Tail, -1, El, TailResult), !.

repl_list([Head|Tail], Col, El, [Head|TailResult]):-
    NewCol is Col - 1,
   	repl_list(Tail, NewCol, El, TailResult).

% repl_matr will replace element in matrix
repl_matr(Matr, Row, Col, El, NewMatr) :-
    nth0(Row, Matr, RowToChange),
    repl_list(RowToChange, Col, El, NewRow),
    repl_list(Matr, Row, NewRow, NewMatr).


overlay(Matr, [], _, _, Matr).

overlay(Matr, [[DeltaX|[DeltaY]]|Tail], CenterX, CenterY, NewMatr) :-
    CheckX is CenterX + DeltaX,
    CheckY is CenterY + DeltaY,
    get_matrix_element(Matr, CheckY, CheckX, Element),
    Element == 0,
    repl_matr(Matr, CheckY, CheckX, 4, ChangedMatr),
    overlay(ChangedMatr, Tail, CenterX, CenterX, NewMatr).
    