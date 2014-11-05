#!/usr/bin/env python
#
#=======================================================================
#             Ludum Test - Block Digger
#=======================================================================
import datetime
import urllib
import webapp2
import random
import json
import os
import jinja2

from google.appengine.ext import db
from google.appengine.api import urlfetch
from google.appengine.api import memcache
from google.appengine.api import users
from google.appengine.ext import blobstore

jinja_enviroment = jinja2.Environment(loader=jinja2.FileSystemLoader(os.path.dirname(__file__)))

class ScoreRecord(db.Model):
	user     = db.UserProperty(auto_current_user=True)
	joined   = db.DateTimeProperty(auto_now_add=True)
	nickname = db.StringProperty(required=True)
	score    = db.IntegerProperty(required=True)
	red      = db.IntegerProperty(required=True)
	blue     = db.IntegerProperty(required=True)
	green    = db.IntegerProperty(required=True)
	yellow   = db.IntegerProperty(required=True)
	purple   = db.IntegerProperty(required=True)


class TestForm(webapp2.RequestHandler):
	def get(self):
		self.response.out.write("""<div class="testform"><h3>Test Form</h3>
				<form action="/" method="post">
				<input type="text" name="jsonText" />
				<input type="submit" value="Submit" />
				</form></div>
				""")

class MainHandler(webapp2.RequestHandler):
    def get(self):
    	self.response.headers['content-type'] = 'text/html; charset=utf-8'
    	self.response.out.write(
    		"Dummy 1" + ",score:" + str(100) + ",red:" + str(34) + ",blue:" + str(34) + ",green:" + str(34) + ",yellow:" + str(34) + ",purple:" + str(34) + "\n" + 
    		"Dummy 2" + ",score:" + str(100) + ",red:" + str(34) + ",blue:" + str(34) + ",green:" + str(34) + ",yellow:" + str(34) + ",purple:" + str(34) + "\n" + 
    		"Dummy 3" + ",score:" + str(100) + ",red:" + str(34) + ",blue:" + str(34) + ",green:" + str(34) + ",yellow:" + str(34) + ",purple:" + str(34) + "\n" + 
    		"Dummy 4" + ",score:" + str(100) + ",red:" + str(34) + ",blue:" + str(34) + ",green:" + str(34) + ",yellow:" + str(34) + ",purple:" + str(34) + "\n" + 
    		"Dummy 5" + ",score:" + str(100) + ",red:" + str(34) + ",blue:" + str(34) + ",green:" + str(34) + ",yellow:" + str(34) + ",purple:" + str(34));

    def post(self):
    	self.response.headers['content-type'] = 'text/html; charset=utf-8'
    	self.response.write(json.dumps(json.loads(self.request.get('jsonText'))))

app = webapp2.WSGIApplication([
    ('/', MainHandler),
    ('/Test', TestForm)
], debug=True)
