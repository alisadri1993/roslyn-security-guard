from jinja2 import Environment, FileSystemLoader
import os
import yaml

THIS_DIR = os.path.dirname(os.path.abspath(__file__))

def render_template(filename,**args):
    print("Generating "+filename)
    j2_env = Environment(loader=FileSystemLoader(THIS_DIR+"/templates"),trim_blocks=True)
    main_html = j2_env.get_template(filename).render(args)
    header_html = j2_env.get_template("header.htm").render(args)
    footer_html = j2_env.get_template("footer.htm").render(args)
    with open(THIS_DIR+"/out_site/"+filename, "wb") as fh:
        fh.write(header_html)
        fh.write(main_html)
        fh.write(footer_html)

#Loading rule descriptions
rules = {}
with open("../RoslynSecurityGuard/RoslynSecurityGuard/Messages.yml", 'r') as stream:
    try:
        data = yaml.load(stream)

        for msg_key in data:
            print "Loading "+msg_key
            #print data[msg_key]
            rules[msg_key] = {}
            rules[msg_key]['Title'] = data[msg_key]['title']
            rules[msg_key]['Message'] = data[msg_key]['description']
    except yaml.YAMLError as exc:
        print(exc)

nb_sinks = 0
with open("../RoslynSecurityGuard/RoslynSecurityGuard/Sinks.yml", 'r') as stream:
    try:
        data = yaml.load(stream)
        nb_sinks = len(data)
        print nb_sinks
    except yaml.YAMLError as exc:
        print(exc)


nb_passwords = 0
with open("../RoslynSecurityGuard/RoslynSecurityGuard/Passwords.yml", 'r') as stream:
    try:
        data = yaml.load(stream)
        nb_passwords = len(data)
        print nb_sinks
    except yaml.YAMLError as exc:
        print(exc)

#Building the complete website

download_link = "https://visualstudiogallery.msdn.microsoft.com/28033821-c053-4652-b529-0bad9c0feba4/referral/232680"
version = '2.0.0'

render_template('index.htm', title='Home' , latest_version=version, nb_rules=len(rules), nb_signatures=(len(rules)+nb_sinks+nb_passwords), download_link = download_link)
render_template('rules.htm', title='Rules', rules=rules)
