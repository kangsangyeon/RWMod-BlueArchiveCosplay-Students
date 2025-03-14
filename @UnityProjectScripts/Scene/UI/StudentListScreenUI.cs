using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UniRx;
using Unity.Linq;
using UnityEngine;

namespace BA
{
    public class StudentListScreenUI : MonoBehaviour
    {
        public StudentListScreenAccessor Accessor;
        private Dictionary<int, ClubAccessor> _clubAccessorById;
        private Dictionary<int, StudentAccessor> _studentAccessorById;

        private void Start()
        {
            _clubAccessorById = new Dictionary<int, ClubAccessor>();
            _studentAccessorById = new Dictionary<int, StudentAccessor>();

            Accessor.ScreenTopBar.BackButton.OnClickAsObservable()
                .Subscribe(_ =>
                {
                    Accessor.gameObject.SetActive(false);
                    Contents.Instance.Accessor.PadCanvas.MainScreen.gameObject.SetActive(true);
                })
                .AddTo(gameObject);

            Accessor.ScreenTopBar.HomeButton.OnClickAsObservable()
                .Subscribe(_ =>
                {
                    Accessor.gameObject.SetActive(false);
                    Contents.Instance.Accessor.PadCanvas.MainScreen.gameObject.SetActive(true);
                })
                .AddTo(gameObject);

            Accessor.ClubHolder.gameObject.Descendants().Destroy();

            foreach (var shcoolPair in GameResource.SchoolTable)
            {
                var clubList = shcoolPair.Value.ClubList.Select(x => GameResource.ClubTable[x]).ToList();
                foreach (var club in clubList)
                {
                    var clubGo = Instantiate(GameResource.ClubPrefab, Accessor.ClubHolder.transform);
                    var clubAccessor = clubGo.GetComponent<ClubAccessor>();

                    clubAccessor.StudentHolder.Descendants().Destroy();
                    clubAccessor.ClubName.text = club.Name;
                    clubAccessor.Logo.sprite = GameResource.SchoolLogoSprites[shcoolPair.Key];
                    // temp: 임시적으로 모든 클럽의 시너지가 비활성화 되어있다고 간주합니다.
                    clubAccessor.SynergyThumbnail.sprite = GameResource.SynergyDeactivatedSprite;

                    _clubAccessorById.Add(club.Id, clubAccessor);

                    var studentList = club.StudentList.Select(x => GameResource.StudentTable[x]).ToList();
                    foreach (var student in studentList)
                    {
                        var studentGo =
                            Instantiate(GameResource.StudentPrefab, clubAccessor.StudentHolder.transform);
                        var studentAccessor = studentGo.GetComponent<StudentAccessor>();

                        studentAccessor.Portrait.sprite =
                            GameResource.StudentPortraitSprites[student.Id];
                        studentAccessor.AttributeBg.color =
                            UIUtilProcedure.GetAttributeColor(student.Attribute);
                        studentAccessor.AttributeBg_Icon.sprite =
                            GameResource.StudentAttributeIconSprites[(int)student.Attribute];
                        studentAccessor.Name.text = student.Name;

                        studentAccessor.Button.OnClickAsObservable()
                            .Subscribe(_ =>
                            {
                                Accessor.gameObject.SetActive(false);
                                Contents.Instance.Accessor.PadCanvas.StudentInfoScreen.gameObject.SetActive(true);
                                var ui = Contents.Instance.Accessor.PadCanvas.StudentInfoScreen.gameObject.GetComponent<StudentInfoScreenUI>();
                                ui.CharId.Value = student.Id;
                            })
                            .AddTo(gameObject);

                        _studentAccessorById.Add(student.Id, studentAccessor);
                    }
                }
            }

            Accessor.SearchInput.OnValueChangedAsObservable()
                .Subscribe(input =>
                {
                    if (input.Length <= 1)
                    {
                        // 검색어가 1글자 이하인 경우, 필터링하지 않습니다.
                        foreach (var pair in _studentAccessorById)
                            pair.Value.OverlayOnSearchFailed.gameObject.SetActive(false);
                    }
                    else
                    {
                        var studentIds = new HashSet<int>();

                        var searchedStudentData =
                            GameResource.StudentTable.Values
                                .Where(x => x.Name != null && x.Name.Contains(input)).ToList();

                        var searchedSchoolData =
                            GameResource.SchoolTable.Values
                                .Where(x => x.Name != null && x.Name.Contains(input)).ToList();

                        var searchedClubData =
                            GameResource.ClubTable.Values
                                .Where(x => x.Name != null && x.Name.Contains(input)).ToList();

                        foreach (var data in searchedSchoolData)
                        {
                            searchedClubData.AddRange(
                                data.ClubList.Select(x => GameResource.ClubTable[x]));
                        }

                        bool onlyOneClubSearched =
                            searchedStudentData.Count == 0 && searchedClubData.Count == 1;

                        foreach (var data in searchedClubData)
                        {
                            foreach (var id in data.StudentList)
                                studentIds.Add(id);
                        }

                        foreach (var data in searchedStudentData)
                            studentIds.Add(data.Id);

                        foreach (var pair in _studentAccessorById)
                        {
                            bool searchFailed = !studentIds.Contains(pair.Key);
                            pair.Value.OverlayOnSearchFailed.gameObject.SetActive(searchFailed);
                        }

                        if (onlyOneClubSearched)
                        {
                            // 하나의 클럽으로 특정된다면, 그 클럽의 위치로 자동 스크롤합니다.
                            var clubData = searchedClubData.First();
                            ScrollTo(clubData.Id);
                        }
                        else if (studentIds.Count == 1)
                        {
                            // 한 명의 캐릭터로 특정된다면, 그 캐릭터의 위치로 자동 스크롤합니다.
                            var studentId = studentIds.First();
                            var clubData = GameResource.ClubTable.Values
                                .FirstOrDefault(x => x.StudentList.Contains(studentId));
                            ScrollTo(clubData.Id);
                        }
                    }
                })
                .AddTo(gameObject);
        }

        private void ScrollTo(int clubId)
        {
            var clubAccessor = _clubAccessorById[clubId];
            int clubOrder = clubAccessor.transform.ChildIndexOfSelf();
            float position = (float)clubOrder / (clubAccessor.transform.parent.childCount - 1);
            position = 1 - position;
            Accessor.ClubScrollRect.DOKill();
            Accessor.ClubScrollRect.DOVerticalNormalizedPos(position, 0.1f);
        }
    }
}