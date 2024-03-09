using System.Linq;
using UnityEngine;

namespace UnityProjectScripts
{
    public class StudentListScreenUI : MonoBehaviour
    {
        public StudentListScreenAccessor accessor;

        private void Start()
        {
            foreach (var _shcoolPair in GameResource.SchoolTable)
            {
                var _clubList = _shcoolPair.Value.ClubList.Select(x => GameResource.ClubTable[x]).ToList();
                foreach (var _clubPair in _clubList)
                {
                    var _clubGo = Instantiate(GameResource.ClubPrefab, accessor.ClubPlaceholder.transform);
                    var _clubAccessor = _clubGo.GetComponent<ClubAccessor>();

                    var _studentList = _clubPair.StudentList.Select(x => GameResource.StudentTable[x]).ToList();
                    foreach (var _student in _studentList)
                    {
                        var _studentGo = Object.Instantiate(GameResource.StudentPrefab, _clubGo.transform);
                        var _studentAccessor = _studentGo.GetComponent<StudentAccessor>();

                        _studentAccessor.Frame.sprite =
                            GameResource.StudentAttributeFrameSprites[(int)_student.Attribute];
                        // _studentAccessor.Thumbnail.sprite =
                        //     GameResource.StudentThumbnailSprites[(int)_student.Id];
                    }
                }
            }
        }
    }
}