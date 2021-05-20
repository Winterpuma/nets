:- module(server,
      [ server/1            % ?Port
      ]).

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
:- use_module(library(http/http_files)).
:- use_module(library(http/http_unix_daemon)).
:- use_module(library(http/http_log)).
:- use_module(library(http/http_dyn_workers)).


:- consult('/app/solutionFinder.pl').


server(Port) :-
	http_server(http_dispatch, [port(Port), workers(16)]).


% Найти ответ
:- http_handler(/, queen, [methods([get,post]),time_limit(6000)]).

queen(_R):-
	myQuery(Ans),
	reply_json(json{answer:Ans}).

queen(_R):-
	format('Content-type: text/plain~n~nNoAnswer', []).


% Загрузка файла через post, используя multipart
:- http_handler(root(upload/Filename), upload(Filename), []).

upload(Filename, Request) :-
	multipart_post_request(Request), !,
	http_read_data(Request, Parts,
		       [ on_filename(save_file)
		       ]),
	memberchk(file=file(OrigFileName, Saved), Parts),
	consult(Saved), % перезагружает существующие предикаты
	format('Content-type: text/plain~n~n'),
	format('Saved your file "~w" into "~w"~n', [OrigFileName, Saved]).

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