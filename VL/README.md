# Video Library Api

This is REST Api for video library project as part of Internship 2019 at Cubic Nis.

## Installation
This guide assumes that the project is being installed on Centos 7 if ran in local environment or any Linux distribution that supports Docker 19.03+ if ran in production environment.
Also PC should have NVidia graphics card and CUDA support.

To run this project you need to install
- NVidia drivers with ```CUDA```

Other steps depend on the environment in which project is run.

### Local environment

#### Backend
FFMPEG, klv2json and fpmatch should be installed in /opt and organized like this:

<img src="readme images/opt.png"
     alt="Markdown Monster icon"
     style="margin-right: 10px;" />

1. Install dotnet core SDK.
2. Clone project
3. Change directory to video-library
4. To setup backend run ```dotnet restore```
5. To build it run ```dotnet build```

##### Mdspproc 
[Bitbucket link for mdspproc. ](https://bitbucket.motiondsp.com/projects/IKENA/repos/ikena-isr/browse/core)

Add the following code to your ```/etc/yum.repos.d/mdsprepo-nis.repo file```

```json
[mdsprepo-nis]
name=MotionDSP local development repository
baseurl=http://192.168.200.233/ikena/
failovermethod=priority
enabled=1
gpgcheck=0

[mdsprepo-nis-mapserver]
name=MotionDSP local development repository - mapserver
baseurl=http://192.168.200.233/mapserver/
failovermethod=priority
enabled=1
gpgcheck=0 
```

After this, you can install any of the rpms via yum: ```$ sudo yum install mdspproc```. You will probably want to disable firewall on your CentOS before using mdspproc: ```$ sudo systemctl stop firewalld && sudo systemctl disable firewalld```

#### Machine learning
To setup ml part, change directory to pytorch-yolo run
```bash
    pip3 install
        opencv-python \
        Flask \
        flask-restful \
    && \
    pip3 install -r requirements.txt
```
Also yolov3-openimages.weights file has to be downloaded.
Detailed guide can be found [here](https://kobrica.github.io/Documentation_Cubic/).

#### Database
1. Install PostgreSQL database
2. Create user ```db_user``` with password ```db_pass```
3. Install PostGIS extension for PostgreSQL
4. In ```video-library``` folder run ```dotnet ef database update``` to initialize database

To run project in local environment run:

```dotnet run``` (in ```video-library``` folder)


### Docker environment
These dependencies should be installed in order to run project properly:
- ```python3```, ```pip3```
- ```Docker```
- ```docker-compose``` (```pip3 install docker-compose```)
- Nvidia drivers with ```CUDA```
- ```nvidia-container-runtime``` (to access GPU in docker container) [GitHub link](https://github.com/NVIDIA/nvidia-container-runtime)

First, we need to setup nvidia runtime to be accessible in docker.
Edit (or create) file ```/etc/docker/daemon.json``` to have this content:
```json
{
    "default-runtime": "nvidia",
    "runtimes": {
        "nvidia": {
            "path": "/usr/bin/nvidia-container-runtime",
            "runtimeArgs": []
        }
    }
}
```
Make folder ```/app``` and clone ```video-library``` project in that folder.

<img src="readme images/app.png"
     alt="Markdown Monster icon"
     style="margin-right: 10px;" />

Also, ```opt``` folder with fpmatch, fpgen and ffmpeg is expected to be in that folder.

<img src="readme images/app-opt.png"
     alt="Markdown Monster icon"
     style="margin-right: 10px;" />
     
To build and run docker containers run ```docker-compose up --build -d```.

Videos and all of their files are stored in ```/var/lib/docker/volumes/video_library_wwwroot/_data```.

Database is stored in ```/var/lib/docker/volumes/video_library_db-data/_data```.

Logs for backend are stored in ```/var/lib/docker/volumes/video_library_logs/_data/```

All of these 3 folders are mounted into docker containers and their content is persistent between two container starts.

Throughout this project we assume that development environment is running project on local machine(without docker) and that production environment is running project in docker containers.

### Notes
Things that have to be manually changed between docker and local environment include:
In nlog.config line 8 should be:
```xml
<variable name="projectDir" value="../../.." />
```
in local environment, and 
```xml
<variable name="projectDir" value=".." />
```

This should be fixed by looking into environment variable called ```ASPNETCORE_ENVIRONMENT``` and determine which directory project lives in and setting that directory in nlog.config.

## Documentation
Documentation about whole project can be found [here](https://docs.google.com/document/d/1IFl4jlQL20EeGLc_YEWO_NZU27vp2Yt8vclcXb1e0bo/edit?ts=5d3834ee#)

Detailed information can be found on endpoint ```/docs/``` in application when built.

On root of application user can take a look into all video files stored on disk.