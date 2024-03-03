from typing import Dict
import json

PATH = "C:\Users\dnbow\OneDrive\Desktop\ClangenNET\ClangenNET.Core\Common\Conditions\condition_got_strings\gain_congenital_condition_strings.json"
DATA: Dict = json.load(PATH)

for i in range(len(DATA)):
    KEY = list(DATA.keys)[i]
    VALUE = list(DATA.values)[i];

    for k in range(len(VALUE)):
        print(f"    {KEY}.{k}: {VALUE}")
        
