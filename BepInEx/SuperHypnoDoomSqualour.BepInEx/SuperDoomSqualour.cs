using CustomizeLib.BepInEx;
using Il2CppInterop.Runtime.Injection;
using Unity.VisualScripting;
using UnityEngine;

namespace SuperDoomSqualour.BepInEx
{
    public class SuperDoomSqualour : MonoBehaviour
    {
        public SuperDoomSqualour() : base(ClassInjector.DerivedConstructorPointer<SuperDoomSqualour>()) => ClassInjector.DerivedConstructorBody(this);

        public SuperDoomSqualour(IntPtr i) : base(i)
        {
        }

        public void Awake()
        {
            plant.DisableDisMix();
        }

        public void SuperAttackZombie()
        {
            for (int i = Board.Instance.zombieArray.Count - 1; i >= 0; i--)
            {
                var z = Board.Instance.zombieArray[i];
                if (z is not null && !z.isMindControlled)
                {
                    if (Lawnf.TravelAdvanced(Buff))
                    {
                        if (z is not null && !z.IsDestroyed())
                        {
                            Board.Instance.SetDoom(Mouse.Instance.GetColumnFromX(z.GameObject().transform.position.x), z.theZombieRow, false, default, default, 1);
                            z.Die(2);
                        }
                    }
                    else
                    {
                        if (z is not null && !z.IsDestroyed())
                            z.TakeDamage(DmgType.Explode, 3600);
                        if (z is not null && !z.IsDestroyed())
                        {
                            z.isDoom = true;
                            z.doomWithPit = false;
                            z.UpdateColor(Zombie.ZombieColor.Doom);
                        }
                    }
                }
            }
            Board.Instance.SetDoom(plant.thePlantColumn, plant.thePlantRow, false, damage: 0);
        }

        public static int Buff { get; set; } = -1;
        public Squalour plant => gameObject.GetComponent<Squalour>();
    }
}