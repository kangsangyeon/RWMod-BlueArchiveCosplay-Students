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
            shutil.copy(filePath, unity_project_scripts_dir_path)
            shutil.copy(filePath, assemblies_dir_path_14)
            shutil.copy(filePath, assemblies_dir_path_15)
