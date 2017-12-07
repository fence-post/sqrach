import textrazor

textrazor.api_key = "2cd060d77a4939b48b0210265f0481d2143da84fb471764215661ab9"

client = textrazor.TextRazor(extractors=["entities", "topics", "words", "phrases", "dependency-trees", "relations", "entailments", "senses"])


with open('dbpediaTypeFilters.txt', 'r') as f:
    typeFilters = f.read().split('\r\n')

with open('textrazor.txt', 'r') as f:
    data = f.read()

client.set_cleanup_mode("raw")
#client.set_cleanup_use_metadata(True)
#client.set_entity_dbpedia_type_filters(typeFilters);
client.set_classifiers(["textrazor_iab", "textrazor_mediatopics","textrazor_newscodes"])
response = client.analyze(data)

def GetRelationInfo(params):
    relationParams = ""
    if(params != None):
        for w in params:
            relationParams += " ".join([o.id for o in w.entities()]) + "(" + w.relation + "), "
            if(w.relation_parent != None and w.relation_parent.params != None and w.relation_parent.params != params):
                relationParams += GetRelationInfo(w.relation_parent.params)
    return relationParams


for iSent in range(0, len(response.sentences())):
    sent = response.sentences()[iSent]
    iPhrase = 0
    for iWord in range(0, len(sent.words)):
        word = sent.words[iWord]
        entities = ""
        entailments = ""
        properties = ""
        relations = ""
        senses = ""
        relationParams = ""
        relation = ""
        relationParamWords = ""
        nounPhrases = ""
        for w in word.noun_phrases:
            nounPhrases += " ".join([o.token for o in w.words]) + ", "
        relationParams += GetRelationInfo(word.relation_params) #" ".join([o.id for o in w.entities()]) + "(" + w.relation + "), "
        for w in word.relation_params:
            relationParamWords += " ".join([o.token for o in w.param_words])
        #    relationParams += GetRelationInfo(w) #" ".join([o.id for o in w.entities()]) + "(" + w.relation + "), "
        for w in word.relations:
            relations += " ".join([o.token for o in w.predicate_words]) + ", "
        for w in word.senses:
            senses += w["synset"] + "(" + str(w["score"]) + ") "
        for w in word.property_predicates:
            for p in w.predicate_words:
                properties += p.token + ", "
        for w in word.entailments:
            entailments += w.entailed_word + "(" + str(w.score) + ") "
        for w in word.entities:
            entities += w.id + " "
        if word.token == ',':
            iPhrase = iPhrase + 1
        else:
            print(iSent, iPhrase, iWord, "token", word.token)
            print(iSent, iPhrase, iWord, "lemma", word.lemma)
            print(iSent, iPhrase, iWord, "stem",  word.stem)
            print(iSent, iPhrase, iWord, "part_of_speech", word.part_of_speech)
            print(iSent, iPhrase, iWord, "pos", word.position)
            print(iSent, iPhrase, iWord, "parent_pos", word.parent_position)
            print(iSent, iPhrase, iWord, "relation_to_parent", word.relation_to_parent)
            if(senses != ""):
                print(iSent, iPhrase, iWord, "senses", senses)
            if(entailments != ""):
                print(iSent, iPhrase, iWord, "entailments", entailments)
            if(entities != ""):
                print(iSent, iPhrase, iWord, "entities", entities)
            if(relationParams != ""):
                print(iSent, iPhrase, iWord, "relationParams", relationParams)
            if(relationParamWords != ""):
                print(iSent, iPhrase, iWord, "relationParamWords", relationParamWords)
            if(relations != ""):
                print(iSent, iPhrase, iWord, "relations", relations)
print(0, 0, 0, "token")
 #if(properties != ""):
            #    print(iSent, iPhrase, iWord, "properties", properties)
            #if(properties != ""):
            #    print(iSent, iPhrase, iWord, "properties", properties)
            
#print("----relations----")
#for iRelation in range(0, len(response.relations())):
#    relation = response.relations()[iRelation]
#    print(" ".join([o.token for o in relation.predicate_words]))
           
           
#print("----categories----")
#for w in response.categories():
#    print(w.label + " " + str(w.score))
      
             
#print("----topics----")
#for w in response.topics():
#    print(w.label + " " + str(w.score))

#print("----properties----")
#for w in response.properties():
#    print("proprty words:" + " ".join([o.token for o in w.property_words]))
#    print("predicate_words:" + " ".join([o.token for o in w.predicate_words]))
      
      