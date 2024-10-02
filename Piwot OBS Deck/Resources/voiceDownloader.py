from gtts import gTTS
import sys

def tts(text:str, save_path:str):
    try:
        audio = gTTS(text=text, lang="pl", slow=False)
        
        audio.save(save_path)
    except Exception as ex:
        print(f"fail: {ex}")
    print("success")


if __name__ == '__main__':
    tts(sys.argv[1], sys.argv[2])