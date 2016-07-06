#
# Copyright 2002 Sun Microsystems, Inc. All rights reserved.
# SUN PROPRIETARY/CONFIDENTIAL. Use is subject to license terms.
#

# @(#)GNUmakefile	1.61 02/01/17

SRC = ../src/com/sun/java/accessibility/util
PKG = com/sun/java/accessibility/util
CLS = classes/$(PKG)
ifeq ($(TOPLEVEL),)
TOPLEVEL = $(shell c:\mksnt\pwd.exe)
endif
CLASS_DEST = $(TOPLEVEL)/classes

include config

CLASSPATH=$(CLASS_DEST)

# set the CLASSPATH separator
ifeq ($(CSEP),)
ifeq ($(OS), Windows_NT)
CSEP = ;
endif
endif
ifeq ($(CSEP),)
CSEP = :
endif

AWTTRANSLATORS = \
	$(CLS)/java/awt/ButtonTranslator.class		\
	$(CLS)/java/awt/CheckboxTranslator.class	\
	$(CLS)/java/awt/LabelTranslator.class		\
	$(CLS)/java/awt/ListTranslator.class		\
	$(CLS)/java/awt/TextComponentTranslator.class

JACCESS = \
	$(CLS)/EventID.class				\
	$(CLS)/GUIInitializedListener.class		\
	$(CLS)/GUIInitializedMulticaster.class		\
	$(CLS)/TopLevelWindowListener.class		\
	$(CLS)/TopLevelWindowMulticaster.class		\
	$(CLS)/Translator.class				\
	$(CLS)/EventQueueMonitor.class			\
	$(CLS)/AccessibilityListenerList.class		\
	$(CLS)/AWTEventMonitor.class			\
	$(CLS)/SwingEventMonitor.class			\
	$(CLS)/AccessibilityEventMonitor.class		\
	$(AWTTRANSLATORS) 

JAVAC = $(JDK_HOME)/bin/javac $(DEPRECATION) -g -d $(CLASS_DEST) -classpath $(CLASS_DEST)
JAR = $(JDK_HOME)/bin/java -ms8m sun.tools.jar.Main

JACCESSJAR = jaccess.jar
EXAMPLESJAR = jaccess-examples.jar

SRC_STAGE = src_stage/com/sun/java
CLS_STAGE = cls_stage/com/sun/java
IMAGES = $(JDKSRC)/src/share/doc/api/images/*.gif

all: $(EXAMPLESJAR)

$(EXAMPLESJAR): $(JACCESSJAR)
	rm -rf examples_classes
	mkdir -p examples_classes
	echo ">>>Recursively making examples..."
	cd ../src/examples; $(MAKE) "JDK_HOME=$(JDK_HOME)" "CSEP=$(CSEP)" || exit 1; cd ../../build
	echo "<<<Finished recursively making examples."
	rm -f $(EXAMPLESJAR)
	find ../src/examples -name "*.class" -exec cp {} examples_classes \;
	(cd examples_classes; $(JAR) cf ../$@ `find . -type f -print`)

$(JACCESSJAR): classes $(JACCESS)
	rm -f $(JACCESSJAR)
	rm -rf cls_stage
	mkdir cls_stage
	cp -r classes/* cls_stage
	(cd cls_stage; $(JAR) cf ../$@ `find com -type f -print`)

examples: 
	rm -f $(EXAMPLESJAR)
	gnumake

classes:
	mkdir -p classes

release: clean release_doc release_dir
	tar cfh $(RELEASE_FILE).tar $(RELEASE_DIR)
	compress $(RELEASE_FILE).tar
	tar cfh $(RELEASE_FILE).tar $(RELEASE_DIR)
	gzip $(RELEASE_FILE).tar
	zip -qr $(RELEASE_FILE).zip $(RELEASE_DIR)
	@echo "finished packaging"

release_doc: $(SRC_STAGE) $(CLS_STAGE)
	rm -rf release_doc
	mkdir -p release_doc/doc/api
	cp ../doc/README.txt ../doc/LICENSE.txt ../doc/NOTES.txt ../doc/CHANGES.txt release_doc
	-CLASSPATH=$(CLASSPATH):cls_stage:src_stage $(JAVADOC) -d release_doc/doc/api -J-ms48m -J-mx48m \
	    com.sun.java.accessibility.util
	cp -r ../doc/api release_doc/doc/
	cp -r ../doc/spec/* release_doc/doc
	(cd ../src/examples; gnumake release)
	(cd release_doc; find . -name SCCS -exec rm -rf {} \; )
	rm -f release_doc/images/*.dsf release_doc/images/*.mic
	rm -f release_doc/images/*.draw release_doc/images/*.tif
	@echo "finished making release documentation"

check_html:
	(cd release_doc/doc; rm -f webtree.fls; \
	 nawk -f ../../webtree index.html; mv webtree.fls ../..)

release_dir: all src.zip
	rm -rf release_dir $(RELEASE_DIR) $(RELEASE_FILE)*
	mkdir release_dir
	ln -s release_dir $(RELEASE_DIR)
	cp -r $(JACCESSJAR) release_dir
	cp -r $(EXAMPLESJAR) release_dir
	cp -r release_doc/* release_dir
	cp src.zip release_dir
	(cd release_dir; find . -name SCCS -exec rm -rf {} \; )
	rm -f release_dir/doc/images/*.dsf release_dir/doc/images/*.mic
	rm -f release_dir/doc/images/*.draw release_dir/doc/images/*.tif
	(cd release_dir; find . -type d -print | nawk \
	'{ if(!system("test -f " $$1 "/private.fls")) system("(cd " $$1 "; rm -f `cat private.fls` private.fls)") }')

src.zip: $(SRC_STAGE)
	ln -s src_stage/com/sun src
	zip -qr src.zip src
	rm src

$(SRC_STAGE):
	mkdir -p $(SRC_STAGE)
	cp -R $(SRC)/.. $(SRC_STAGE)
	(cd src_stage; find . -name SCCS -type d -exec rm -rf {} \; ; rm -f $(PRIVATE_CLASSES:.class=.java))

$(CLS_STAGE): classes $(JACCESS)
	mkdir -p $(CLS_STAGE)
	cp -R $(CLS)/.. $(CLS_STAGE)
	(cd cls_stage; rm -f $(PRIVATE_CLASSES))

clean clobber:
	rm -rf classes examples_classes $(JACCESSJAR) $(EXAMPLESJAR) src_stage cls_stage doc release src.zip
	rm -rf src_stage cls_stage release_doc
	rm -rf release_dir $(RELEASE_DIR) $(RELEASE_FILE)*
	cd ../src/examples; $(MAKE) $@ || exit 1; cd ../../build
	cd ..; find . -name \*~ -exec rm {} \; ; cd build
       
classes/%.class: ../src/%.java
	(cd $(?D); $(JAVAC) $(?F))

classes/%.properties: ../src/%.properties
	rm -f $@
	cp $? $@

$(CLS)/%.gif: $(SRC)/%.gif
	@mkdir -p `dirname $@`
	rm -f $@
	cp $? $@

