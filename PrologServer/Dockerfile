FROM swipl

COPY . /app
EXPOSE 8888

ENTRYPOINT ["swipl"]
CMD ["/app/server.pl",  "--user=daemon", "--fork=false", "--port=8888"]
