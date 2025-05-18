<template>
        <div id="mymap"></div>
</template>

<script>
import L from 'leaflet';
import {bus} from '../../main.js';
import axios from 'axios';
export default {
    name: 'vue-map',

    data()
    {
        return{
            cordinatesLine:null,
            cordinatesPolygon:null,
            mymap:null
        }
    },
    created()
    {
     bus.$on("updateMap", data => this.getCordinates(data))
    },
    mounted() {
        this.initMap();
    },
    methods: {
        initMap() {
             this.mymap = L.map('mymap').setView([44, 21], 5);
            L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png?').addTo(this.mymap);
        },

        getCordinates(data)
        {
        axios.get(process.env.VUE_APP_URL+'/api/videos/'+data.id+'/geolocation/lat-long')
            .then(respone => {
                if(respone.status==200){
                    if(respone.data.cameraLine != null)
                        this.cordinatesLine = respone.data.cameraLine.coordinates;
                    if(respone.data.filmedArea != null)
                        this.cordinatesPolygon = respone.data.filmedArea.coordinates;

                    this.draw();
                }
            }).catch(error =>
            {
                error;//
                if(this.mymap == null)
                    this.mymap = L.map('mymap').setView([44, 21], 5);

                L.tileLayer('http://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png').addTo(this.mymap);
            })
                   
        },
    draw()
    {
        if(this.mymap === null)
        {
            this.mymap = L.map('mymap').setView([44, 21], 4);
            L.tileLayer('http://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png').addTo(this.mymap); 
        }

        if(this.cordinatesLine!=null)
        {
            var line = L.polyline(this.cordinatesLine, {color: 'red'}).addTo(this.mymap);
            this.mymap.fitBounds(line.getBounds());
        }

        if(this.cordinatesPolygon!=null)
        {
            var polyline = L.polygon(this.cordinatesPolygon, {color: 'blue'}).addTo(this.mymap);
            this.mymap.fitBounds(polyline.getBounds());
        }

    }
}
}
</script>

<style>
#mymap {

    display:flex;
    padding: 4px;
    margin-bottom: 5px;
    min-width: 90%;
    min-height: 300px; 
    align-self: center;
}
</style>