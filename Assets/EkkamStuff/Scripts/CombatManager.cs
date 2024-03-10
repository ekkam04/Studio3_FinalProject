using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

namespace Ekkam
{
    public class CombatManager : MonoBehaviour
    {
        Animator anim;
        Rigidbody rb;

        [SerializeField] GameObject arrow;
        [SerializeField] GameObject spellBall;
        [SerializeField] GameObject meleeHitbox;

        [SerializeField] GameObject itemHolderLeft;
        [SerializeField] GameObject itemHolderRight;

        public LayerMask layersToIgnore;

        void Start()
        {
            anim = GetComponent<Animator>();
            rb = GetComponent<Rigidbody>();
            
            var meleeHitboxCollider = meleeHitbox.GetComponent<Collider>();
            meleeHitboxCollider.excludeLayers = layersToIgnore;
        }

        void Update()
        {
            
        }

        public async void MeleeAttack()
            {
                anim.SetTrigger("swordAttack");
                anim.SetLayerWeight(1, 0);
                await Task.Delay(250);
                meleeHitbox.SetActive(true);
                rb.AddForce(transform.forward * 3.5f, ForceMode.Impulse);
                await Task.Delay(50);
                meleeHitbox.SetActive(false);
            }
            
            public async void ArcherAttack()
            {
                anim.SetTrigger("bowAttack");
                await Task.Delay(250);
                
                GameObject newArrow = Instantiate(arrow, itemHolderLeft.transform.position, Quaternion.identity, itemHolderRight.transform);
                newArrow.transform.localRotation = Quaternion.identity;
                newArrow.transform.Rotate(0, 90, 0);
                newArrow.transform.localPosition = new Vector3(0, 0, 0);
                newArrow.SetActive(true);
                await Task.Delay(550);
                
                newArrow.transform.SetParent(null);
                newArrow.transform.forward = transform.forward;
                newArrow.GetComponent<Projectile>().speed = 15;
                await Task.Delay(100);
                newArrow.GetComponent<Collider>().enabled = true;
            }
            
            public async void MageAttack()
            {
                anim.SetTrigger("swordAttack");
                await Task.Delay(250);
                GameObject newSpellBall = Instantiate(spellBall, transform.position + transform.forward + new Vector3(0, 1, 0), Quaternion.identity);
                newSpellBall.transform.forward = transform.forward;
                newSpellBall.SetActive(true);
            }
    }
}
