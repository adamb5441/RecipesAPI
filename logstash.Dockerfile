FROM logstash:5.4.3

WORKDIR /usr/share/logstash

RUN bin/logstash-plugin install logstash-input-mongodb \ 
      && bin/logstash -f /etc/logstash/conf.d/mongodata.conf