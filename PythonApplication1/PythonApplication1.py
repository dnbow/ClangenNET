from typing import Dict
import json
import os

PATH = r"C:\Users\dnbow\OneDrive\Desktop\ClangenNET\ClangenNET.Core\Common\Events\Ceremonies\Master.json"

def PC(string):
    return ''.join(x for x in string.title() if not x.isspace() and x != "_");

DATA = json.loads(open(PATH, "r").read())


for (ID, DATA) in DATA.items():
    print(f"    ClanCeremony.{PC(ID)}: \"{DATA[1]}\"")
    




        

        