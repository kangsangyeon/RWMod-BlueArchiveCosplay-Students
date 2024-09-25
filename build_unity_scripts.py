import os
import shutil
import subprocess
from pathlib import Path

# 리소스 타입 확장자 목록 설정
resource_types = set['.mat', '.mat.meta', '.shader', '.mat.meta']

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
unity_project_plugin_resources_dir_path = './@UnityProject/Assets/@MyProject/Resources/PluginResources'

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
    # 디렉토리 명에서 plugin_name을 구함.
    normalized_path = os.path.normpath(root)
    path_tokens = normalized_path.split(os.sep)
    plugin_name = ''
    dir_path_under_plugin_dir = ''
    try:
        plugins_token_idx = path_tokens.index('Plugins')
        plugin_name = path_tokens[plugins_token_idx + 1]
        dir_path_under_plugin_dir = os.path.sep.join(path_tokens[plugins_token_idx + 2])
    except Exception as e:
        pass
    # 파일을 적절한 위치에 복사함.
    for file in files:
        file_path = f'{root}/{file}'
        if file.endswith('.dll'):
            shutil.copy(file_path, unity_project_scripts_dir_path)
            shutil.copy(file_path, assemblies_dir_path_14)
            shutil.copy(file_path, assemblies_dir_path_15)
        else:
            _, ext = os.path.splitext(file)
            if ext not in resource_types:
                continue
            Path(f'{unity_project_plugin_resources_dir_path}/{plugin_name}').mkdir(parents=True, exist_ok=True)
            destination_file_path = f'{unity_project_plugin_resources_dir_path}/{plugin_name}/{dir_path_under_plugin_dir}/{file}'
            shutil.copy(file_path, destination_file_path)
