using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using UnityEngine.Animations.Rigging;

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
        
        [SerializeField] GameObject target;
        [SerializeField] private float targetHeightOffset;
        
        public static CombatManager CurrentAttacker = null;

        public LayerMask layersToIgnore;

        void Start()
        {
            anim = GetComponent<Animator>();
            rb = GetComponent<Rigidbody>();
            
            if (meleeHitbox != null)
            {
                var meleeHitboxCollider = meleeHitbox.GetComponent<Collider>();
                meleeHitboxCollider.excludeLayers = layersToIgnore;
                meleeHitbox.SetActive(false);
            }
            
            if (GetComponent<Enemy>() != null)
            {
                target = Player.Instance.gameObject;
            }
        }

        void Update()
        {
            
        }

        public async void MeleeAttack()
        {
            if (GetComponent<Enemy>())
            {
                if (CurrentAttacker != null && CurrentAttacker != this) return;
                
                SoundManager.Instance.PlaySound("axe-swing");
                CurrentAttacker = this;
                anim.SetTrigger("swordAttack");
                
                if (anim.layerCount > 1) anim.SetLayerWeight(1, 0);
                await Task.Delay(250);
                meleeHitbox.SetActive(true);
                rb.AddForce(transform.forward * 3.5f, ForceMode.Impulse);
                await Task.Delay(50);
                meleeHitbox.SetActive(false);
                CurrentAttacker = null;
            }
            else
            {
                SoundManager.Instance.PlaySound("sword-swing");
                var particleSystem = itemHolderRight.GetComponentInChildren<ParticleSystem>();
                anim.SetTrigger("swordAttack");
                if (particleSystem != null) particleSystem.Play();
                // get all layers
                if (anim.layerCount > 1) anim.SetLayerWeight(1, 0);
                await Task.Delay(250);
                meleeHitbox.SetActive(true);
                rb.AddForce(transform.forward * 3.5f, ForceMode.Impulse);
                await Task.Delay(50);
                meleeHitbox.SetActive(false);
                if (particleSystem != null) particleSystem.Stop();
            }
        }
        
        public async void ArcherAttack(Item bow, Player player)
        {
            var mainCamera = Camera.main;
            // transform.forward = new Vector3(mainCamera.transform.forward.x, 0, mainCamera.transform.forward.z);
            player.secondHandArrowIKWeight = 1;
            
            anim.SetTrigger("bowAttack");
            await Task.Delay(250);
            
            var arrowHolder = bow.gameObject.transform.GetChild(0);
            GameObject newArrow = Instantiate(arrow, arrowHolder.position, Quaternion.identity, arrowHolder);
            var arrowCollider = newArrow.GetComponent<Collider>();
            arrowCollider.excludeLayers = layersToIgnore;
            newArrow.transform.localRotation = Quaternion.identity;
            newArrow.SetActive(true);
            await Task.Delay(550);
            
            SoundManager.Instance.PlaySound("arrow-shoot");
            newArrow.transform.SetParent(null);
            newArrow.transform.position = bow.transform.position;
            newArrow.GetComponent<Projectile>().speed = 15;
            newArrow.GetComponent<Projectile>().projectileOwner = GetComponent<Damagable>();
            await Task.Delay(100);
            newArrow.GetComponent<Collider>().enabled = true;
            player.secondHandArrowIKWeight = 0;
        }
        
        public async void MageAttack()
        {
            anim.SetTrigger("staffAttack");
            SoundManager.Instance.PlaySound("staff-cast");
            await Task.Delay(250);
            GameObject newSpellBall = Instantiate(spellBall, transform.position + transform.forward + new Vector3(0, 1, 0), Quaternion.identity);
            newSpellBall.transform.LookAt(target.transform.position + new Vector3(0, targetHeightOffset, 0));
            newSpellBall.GetComponent<Projectile>().projectileOwner = GetComponent<Damagable>();
            var spellBallCollider = newSpellBall.GetComponent<Collider>();
            spellBallCollider.excludeLayers = layersToIgnore;
            newSpellBall.SetActive(true);
            SoundManager.Instance.PlaySound("staff-shoot");
        }
    }
}
