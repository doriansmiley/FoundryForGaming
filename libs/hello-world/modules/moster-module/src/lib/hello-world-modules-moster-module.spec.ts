import { monsterBuilder } from './hello-world-modules-moster-module';
import * as flatbuffers from 'flatbuffers';
import { Weapon, Monster } from './flatbuffers';

describe('monsterBuilder', () => {
  it('should serialize a monster', () => {
    const monsterBuffer = monsterBuilder({
      weapons: [{ name: 'Sword', damage: 5 }],
    });
    expect(monsterBuffer.length).toEqual(72);
  });

  it('should hydrate a monster', () => {
    const monsterBuffer = monsterBuilder({
      weapons: [{ name: 'Sword', damage: 5 }],
    });
    const buf = new flatbuffers.ByteBuffer(monsterBuffer);
    const monster = Monster.getRootAsMonster(buf);
    expect(monster.weaponsLength()).toEqual(1);
    expect(monster.weapons(0)?.name()).toEqual('Sword');
    expect(monster.weapons(0)?.damage()).toEqual(5);
  });
});
