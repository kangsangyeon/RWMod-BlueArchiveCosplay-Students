using System.Collections.Generic;
using System.Linq;
using UniRx;
using UniRx.Triggers;
using Unity.Linq;
using UnityEngine;

namespace UnityProjectScripts
{
    public class StudentListScreenUI : MonoBehaviour
    {
        public StudentListScreenAccessor accessor;
        private PadAccessor padAccessor;
        private Dictionary<int, StudentAccessor> studentAccessorById;

        private void Start()
        {
            padAccessor = FindObjectOfType<PadAccessor>();
            studentAccessorById = new Dictionary<int, StudentAccessor>();

            accessor.ScreenTopBar.BackButton.OnClickAsObservable()
                .Subscribe(_ =>
                {
                    accessor.gameObject.SetActive(false);
                    padAccessor.MainScreen.gameObject.SetActive(true);
                })
                .AddTo(gameObject);

            accessor.ScreenTopBar.HomeButton.OnClickAsObservable()
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

                        _studentAccessor.Frame.OnPointerClickAsObservable()
                            .Subscribe(_ =>
                            {
                                accessor.gameObject.SetActive(false);
                                padAccessor.StudentInfoScreen.gameObject.SetActive(true);
                                var _ui = padAccessor.StudentInfoScreen.gameObject.GetComponent<StudentInfoScreenUI>();
                                _ui.CharId.Value = _student.Id;
                            })
                            .AddTo(gameObject);

                        studentAccessorById.Add(_student.Id, _studentAccessor);
                    }
                }
            }

            accessor.SearchInput.OnValueChangedAsObservable()
                .Subscribe(input =>
                {
                    if (input.Length <= 1)
                    {
                        // 검색어가 1글자 이하인 경우, 필터링하지 않습니다.
                        foreach (var _pair in studentAccessorById)
                            _pair.Value.OverlayOnSearchFailed.gameObject.SetActive(false);
                    }
                    else
                    {
                        var _studentIds = new HashSet<int>();

                        var _searchedStudentData =
                            GameResource.StudentTable.Values
                                .Where(x => x.Name.Contains(input)).ToList();

                        var _searchedSchoolData =
                            GameResource.SchoolTable.Values
                                .Where(x => x.Name.Contains(input)).ToList();

                        var _searchedClubData =
                            GameResource.ClubTable.Values
                                .Where(x => x.Name.Contains(input)).ToList();

                        foreach (var _data in _searchedSchoolData)
                        {
                            _searchedClubData.AddRange(
                                _data.ClubList.Select(x => GameResource.ClubTable[x]));
                        }

                        foreach (var _data in _searchedClubData)
                        {
                            foreach (var _id in _data.StudentList)
                                _studentIds.Add(_id);
                        }

                        foreach (var _data in _searchedStudentData)
                            _studentIds.Add(_data.Id);

                        foreach (var _pair in studentAccessorById)
                        {
                            bool _searchFailed = !_studentIds.Contains(_pair.Key);
                            _pair.Value.OverlayOnSearchFailed.gameObject.SetActive(_searchFailed);
                        }
                    }
                })
                .AddTo(gameObject);
        }
    }
}