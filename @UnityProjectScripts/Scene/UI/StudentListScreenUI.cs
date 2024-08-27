using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
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
        private Dictionary<int, ClubAccessor> clubAccessorById;
        private Dictionary<int, StudentAccessor> studentAccessorById;

        private void Start()
        {
            padAccessor = FindObjectOfType<PadAccessor>();
            clubAccessorById = new Dictionary<int, ClubAccessor>();
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
                foreach (var _club in _clubList)
                {
                    var _clubGo = Instantiate(GameResource.ClubPrefab, accessor.ClubHolder.transform);
                    var _clubAccessor = _clubGo.GetComponent<ClubAccessor>();

                    _clubAccessor.StudentHolder.Descendants().Destroy();
                    _clubAccessor.ClubName.text = _club.Name;
                    _clubAccessor.Logo.sprite = GameResource.SchoolLogoSprites[_shcoolPair.Key];
                    // temp: 임시적으로 모든 클럽의 시너지가 비활성화 되어있다고 간주합니다.
                    _clubAccessor.SynergyThumbnail.sprite = GameResource.SynergyDeactivatedSprite;

                    clubAccessorById.Add(_club.Id, _clubAccessor);

                    var _studentList = _club.StudentList.Select(x => GameResource.StudentTable[x]).ToList();
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
                                .Where(x => x.Name != null && x.Name.Contains(input)).ToList();

                        var _searchedSchoolData =
                            GameResource.SchoolTable.Values
                                .Where(x => x.Name != null && x.Name.Contains(input)).ToList();

                        var _searchedClubData =
                            GameResource.ClubTable.Values
                                .Where(x => x.Name != null && x.Name.Contains(input)).ToList();

                        foreach (var _data in _searchedSchoolData)
                        {
                            _searchedClubData.AddRange(
                                _data.ClubList.Select(x => GameResource.ClubTable[x]));
                        }

                        bool _onlyOneClubSearched =
                            _searchedStudentData.Count == 0 && _searchedClubData.Count == 1;

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

                        if (_onlyOneClubSearched)
                        {
                            // 하나의 클럽으로 특정된다면, 그 클럽의 위치로 자동 스크롤합니다.
                            var _clubData = _searchedClubData.First();
                            ScrollTo(_clubData.Id);
                        }
                        else if (_studentIds.Count == 1)
                        {
                            // 한 명의 캐릭터로 특정된다면, 그 캐릭터의 위치로 자동 스크롤합니다.
                            var _studentId = _studentIds.First();
                            var _clubData = GameResource.ClubTable.Values
                                .FirstOrDefault(x => x.StudentList.Contains(_studentId));
                            ScrollTo(_clubData.Id);
                        }
                    }
                })
                .AddTo(gameObject);
        }

        private void ScrollTo(int _clubId)
        {
            var _clubAccessor = clubAccessorById[_clubId];
            int _clubOrder = _clubAccessor.transform.ChildIndexOfSelf();
            float _position = (float)_clubOrder / (_clubAccessor.transform.parent.childCount - 1);
            _position = 1 - _position;
            accessor.ClubScrollRect.DOKill();
            accessor.ClubScrollRect.DOVerticalNormalizedPos(_position, 0.1f);
        }
    }
}