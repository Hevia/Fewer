import zipfile
import os

def zip_files(file_paths, zip_name):
    with zipfile.ZipFile(zip_name, 'w', zipfile.ZIP_DEFLATED) as zipf:
        for file_path in file_paths:
            if os.path.isfile(file_path):
                zipf.write(file_path, os.path.basename(file_path))
            elif os.path.isdir(file_path):
                for root, dirs, files in os.walk(file_path):
                    for file in files:
                        full_path = os.path.join(root, file)
                        relative_path = os.path.relpath(full_path, file_path)
                        zipf.write(full_path, os.path.join(os.path.basename(file_path), relative_path))

# Example usage
folder_paths = ['plugins', 'icon.png', 'manifest.json', 'README.md']  # List of folder paths to be zipped
zip_name = 'FewArtifact.zip'  # Name of the zip file to be created

zip_files(folder_paths, zip_name)
