import keyboard
import random
from asyncio import create_task, get_event_loop, sleep
import mouse

keys = ['a', 'd', 'space', 'mouse']
tasks = set()

loop = get_event_loop()
async def RandomInput():
    key = random.choice(keys)
    if key == 'mouse':
        x = random.randint(240, 1666)
        y = random.randint(240, 980)
        mouse.move(x, y)
        mouse.click()
    else:
        keyboard.press(key)
    x = random.randint(1,  2)
    await sleep(x)
    keyboard.release(key)


async def run(l):
    while True:
        tasks.add(l.create_task(RandomInput()))
        await sleep(1)

loop.run_until_complete(run(loop))
loop.close()
