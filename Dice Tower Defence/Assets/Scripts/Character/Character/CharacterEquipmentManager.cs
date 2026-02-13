using UnityEngine;

namespace UB
{
    public class CharacterEquipmentManager : MonoBehaviour
    {
        private CharacterManager characterManager;

        public WeaponModelInstantiationSlot RightHandSlot;
        public WeaponModelInstantiationSlot LeftHandSlot;

        public GameObject RightHandWeaponModel;
        public GameObject LeftHandWeaponModel;

        protected virtual void Awake()
        {

        }

        protected virtual void Start()
        {
            characterManager = GetComponent<CharacterManager>();
            // Get our slots
            InitializeWeaponSlots();
            // Load weapons on both hands
            LoadWeaponsOnBothHands();
        }

        private void InitializeWeaponSlots()
        {
            WeaponModelInstantiationSlot[] weaponSlots = GetComponentsInChildren<WeaponModelInstantiationSlot>();
            foreach (var weaponSlot in weaponSlots) {
                if (weaponSlot.WeaponSlot == WeaponModelSlot.RightHand) {
                    RightHandSlot = weaponSlot;
                }
                else if (weaponSlot.WeaponSlot == WeaponModelSlot.LeftHand) {
                    LeftHandSlot = weaponSlot;
                }
            }
        }

        public void LoadWeaponsOnBothHands()
        {
            LoadRightWeapon();
            LoadLeftWeapon();
        }

        public void LoadRightWeapon()
        {
            if (characterManager.CharacterInventoryManager.CurrentRightHandWeapon != null) {
                RightHandWeaponModel = Instantiate(characterManager.CharacterInventoryManager.CurrentRightHandWeapon.WeaponModel);
                RightHandSlot.LoadWeaponModel(RightHandWeaponModel);
            }
        }

        public void LoadLeftWeapon()
        {
            if (characterManager.CharacterInventoryManager.CurrentLeftHandWeapon != null) {
                LeftHandWeaponModel = Instantiate(characterManager.CharacterInventoryManager.CurrentLeftHandWeapon.WeaponModel);
                LeftHandSlot.LoadWeaponModel(LeftHandWeaponModel);
            }
        }

        protected virtual void OnDestroy()
        {
        }
    }
}
