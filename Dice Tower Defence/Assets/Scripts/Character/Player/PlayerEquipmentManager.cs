using Unity.VisualScripting;
using UnityEngine;

namespace UB
{
    public class PlayerEquipmentManager : CharacterEquipmentManager
    {
        private PlayerManager playerManager;
        public DieModelInstantiationSlot DieHeadSlot;
        public GameObject DieHeadModel;
        public DieController DieController;

        protected override void Awake()
        {
            base.Awake();
        }

        protected override void Start()
        {
            base.Start();

            playerManager = GetComponent<PlayerManager>();

            InitializeDieSlot();
            LoadDieHead();
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

                DieController = DieHeadModel.GetComponentInChildren<DieController>();
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
        }
    }
}
