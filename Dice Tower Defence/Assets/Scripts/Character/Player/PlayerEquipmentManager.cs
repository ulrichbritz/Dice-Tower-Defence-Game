using Unity.VisualScripting;
using UnityEngine;

namespace UB
{
    public class PlayerEquipmentManager : CharacterEquipmentManager
    {
        private PlayerManager playerManager;

        public WeaponModelInstantiationSlot RightHandSlot;
        public WeaponModelInstantiationSlot LeftHandSlot;
        public DieModelInstantiationSlot DieHeadSlot;

        public GameObject RightHandWeaponModel;
        public GameObject LeftHandWeaponModel;
        public GameObject DieHeadModel;

        protected override void Awake()
        {
            base.Awake();
        }

        protected override void Start()
        {
            base.Start();

             base.Awake();

            playerManager = GetComponent<PlayerManager>();
            // Get our slots
            InitializeWeaponSlots();
            InitializeDieSlot();

            // Load weapons on both hands
            LoadWeaponsOnBothHands();
            LoadDieHead();
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
            if (playerManager.PlayerInventoryManager.CurrentRightHandWeapon != null) {
                RightHandWeaponModel = Instantiate(playerManager.PlayerInventoryManager.CurrentRightHandWeapon.WeaponModel);
                RightHandSlot.LoadWeaponModel(RightHandWeaponModel);
            }
        }

        public void LoadLeftWeapon()
        {
            if (playerManager.PlayerInventoryManager.CurrentLeftHandWeapon != null) {
                LeftHandWeaponModel = Instantiate(playerManager.PlayerInventoryManager.CurrentLeftHandWeapon.WeaponModel);
                LeftHandSlot.LoadWeaponModel(LeftHandWeaponModel);
            }
        }

        private void InitializeDieSlot()
        {
            DieModelInstantiationSlot dieSlot = GetComponentInChildren<DieModelInstantiationSlot>();
            if (dieSlot != null) {
                DieHeadSlot = dieSlot;
            }
        }

        public void LoadDieHead()
        {
            if (playerManager.PlayerInventoryManager.CurrentDieHead != null) {
                DieHeadModel = Instantiate(playerManager.PlayerInventoryManager.CurrentDieHead.DieModel);
                DieHeadSlot.LoadDieModel(DieHeadModel);
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
        }
    }
}
