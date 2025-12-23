using UnityEngine;

namespace UB
{
    public class DieModelInstantiationSlot : MonoBehaviour
    {
        public GameObject CurrentDieModel;

        public void UnloadDieModel()
        {
            if (CurrentDieModel != null) {
                Destroy(CurrentDieModel);
            }
        }

        public void LoadDieModel(GameObject dieModel)
        {
            CurrentDieModel = dieModel;
            dieModel.transform.parent = transform;

            dieModel.transform.localPosition = Vector3.zero;
            dieModel.transform.localRotation = Quaternion.identity;
            dieModel.transform.localScale = Vector3.one;
        }
    }
}
