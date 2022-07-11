import * as flatbuffers from 'flatbuffers';
import { Weapon, Monster } from './flatbuffers';

export type WeaponProps = {
  name: string;
  damage: number;
};

export function monsterBuilder({
  weapons,
}: {
  weapons: WeaponProps[];
}): Uint8Array {
  const builder = new flatbuffers.Builder(1024);
  const buffers = weapons.map((weapon) => {
    const name = builder.createString(weapon.name);
    Weapon.startWeapon(builder);
    Weapon.addName(builder, name);
    Weapon.addDamage(builder, weapon.damage);
    return Weapon.endWeapon(builder);
  });
  const weaponsVector = Monster.createWeaponsVector(builder, buffers);
  Monster.startMonster(builder);
  Monster.addWeapons(builder, weaponsVector);
  const monster = Monster.endMonster(builder);
  builder.finish(monster);
  return builder.asUint8Array();
}
