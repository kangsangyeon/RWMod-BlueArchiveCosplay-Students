import os
import shutil
import subprocess
from pathlib import Path

# 디렉토리 설정
configuration = 'Debug'
project_name = '@UnityProjectScripts'
build_assembly_name = 'UnityProjectScripts'
project_file_path = f'./{project_name}/{project_name}.csproj'
build_dll_file_path = f'./{project_name}/bin/{configuration}/{build_assembly_name}.dll'
build_pdb_file_path = f'./{project_name}/bin/{configuration}/{build_assembly_name}.pdb'
assemblies_dir_path_14 = './1.4/Assemblies'
assemblies_dir_path_15 = './1.5/Assemblies'
unity_project_scripts_dir_path = './@UnityProject/Assets/@MyProject/Scripts/Runtime'

# 라이브러리를 빌드합니다.
subprocess.run(['dotnet', 'build', project_file_path, '-property', f'Configuration={configuration}'])

# Assemblies 폴더가 없다면 만듭니다.
Path(assemblies_dir_path_14).mkdir(parents=True, exist_ok=True)
Path(assemblies_dir_path_15).mkdir(parents=True, exist_ok=True)

# Assemblies 폴더와 Unity Project Scripts 폴더로 복사합니다.
shutil.copy(build_dll_file_path, assemblies_dir_path_14)
shutil.copy(build_pdb_file_path, assemblies_dir_path_14)
shutil.copy(build_dll_file_path, assemblies_dir_path_15)
shutil.copy(build_pdb_file_path, assemblies_dir_path_15)
shutil.copy(build_dll_file_path, unity_project_scripts_dir_path)
shutil.copy(build_pdb_file_path, unity_project_scripts_dir_path)

# plugins 폴더 내 모든 dll 파일을 찾아서 Unity Project Scripts 폴더와 Assemblies 폴더로 복사합니다.
for root, dirs, files in os.walk(f'./{project_name}/Plugins'):
    for file in files:
        if file.endswith('.dll'):
            filePath = f'{root}/{file}'
            # netstandard는 UnityProject로 복사하지 않습니다.
            # 하지만 UnityProject에는 이미 2.1.0.0 파일이 내장되어 있으며 UnityProject에 복사할 경우 에러가 발생합니다.
            # Json.Net가 netstandard 2.0.0.0 파일을 필요로 하기 때문에 Assemblies 폴더에는 추가되어야 합니다.
            if file != 'netstandard.dll':
                shutil.copy(filePath, unity_project_scripts_dir_path)
            shutil.copy(filePath, assemblies_dir_path_14)
            shutil.copy(filePath, assemblies_dir_path_15)
