import keyboard
import random
from asyncio import create_task, get_event_loop, sleep
import mouse
import os, sys
from pathlib import Path

keys = ['a', 'd', 'space']
tasks = set()

config = {
    'N': range(5),
    'lag': [
        0,
        50,
        100,
        150,
        200,
        250,
    ],
    'loss': [
        0,
        10,
    ],
}

sec_per_run = 600

loop = get_event_loop()


start_pos = (1452, 452)
lag_pos = (1952, 522)
loss_pos = (1952, 552)


async def RandomInput():
    key = random.choice(keys)
    keyboard.press(key)
    x = random.randint(1, 2)
    await sleep(x)
    keyboard.release(key)


async def run(l):
    for loss in config['loss']:
        for lag in config['lag']:
            for n in config['N']:
                print(f'Running on {lag}ms, {loss}% loss, iter {n}', file=sys.stderr)
                counter = 0
                while counter < sec_per_run:
                    tasks.add(l.create_task(RandomInput()))
                    x = random.randint(240, 1666)
                    y = random.randint(680, 980)
                    mouse.move(x, y)
                    mouse.click()
                    await sleep(1.5)
                    counter += 1

                p = f'Results/NoComp/{lag}ms_{loss}loss'
                Path(p).mkdir(exist_ok=True, parents=True)
                Path('Build/pos0.log').rename(f'{p}/pos_loop{n}.log')
                Path('Build/hit0.log').rename(f'{p}/hit_loop{n}.log')
            input("Continue")


loop.run_until_complete(run(loop))
loop.close()
