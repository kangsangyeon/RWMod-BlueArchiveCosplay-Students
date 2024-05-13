using System.Linq;
using UniRx;
using Unity.Linq;
using UnityEngine;

namespace UnityProjectScripts
{
    public class StudentListScreenUI : MonoBehaviour
    {
        public StudentListScreenAccessor accessor;
        private PadAccessor padAccessor;

        private void Start()
        {
            padAccessor = FindObjectOfType<PadAccessor>();

            accessor.BackButton.OnClickAsObservable()
                .Subscribe(_ =>
                {
                    accessor.gameObject.SetActive(false);
                    padAccessor.MainScreen.gameObject.SetActive(true);
                })
                .AddTo(gameObject);

            accessor.ClubHolder.gameObject.Descendants().Destroy();

            foreach (var _shcoolPair in GameResource.SchoolTable)
            {
                var _clubList = _shcoolPair.Value.ClubList.Select(x => GameResource.ClubTable[x]).ToList();
                foreach (var _clubPair in _clubList)
                {
                    Debug.Log($"{_clubPair.Id}: {_clubPair.Name}");

                    var _clubGo = Instantiate(GameResource.ClubPrefab, accessor.ClubHolder.transform);
                    var _clubAccessor = _clubGo.GetComponent<ClubAccessor>();

                    _clubAccessor.StudentHolder.Descendants().Destroy();
                    _clubAccessor.ClubName.text = _clubPair.Name;
                    _clubAccessor.Logo.sprite = GameResource.SchoolLogoSprites[_shcoolPair.Key];
                    // temp: 임시적으로 모든 클럽의 시너지가 비활성화 되어있다고 간주합니다.
                    _clubAccessor.SynergyThumbnail.sprite = GameResource.SynergyDeactivatedSprite;

                    var _studentList = _clubPair.StudentList.Select(x => GameResource.StudentTable[x]).ToList();
                    foreach (var _student in _studentList)
                    {
                        var _studentGo =
                            Instantiate(GameResource.StudentPrefab, _clubAccessor.StudentHolder.transform);
                        var _studentAccessor = _studentGo.GetComponent<StudentAccessor>();

                        _studentAccessor.Frame.sprite =
                            GameResource.StudentAttributeFrameSprites[(int)_student.Attribute];
                        _studentAccessor.Portrait.sprite =
                            GameResource.StudentPortraitSprites[_student.Id];
                    }
                }
            }
        }
    }
}