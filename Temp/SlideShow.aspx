<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="SlideShow.aspx.cs" Inherits="Default2" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" Runat="Server">

    <div class="slider-frame">
        <div class="slider-images">
            <div class="img-container">
                <img src="https://wonderfulengineering.com/wp-content/uploads/2016/01/Desktop-Background-8.jpg"/>
            </div>
            <div class="img-container">
                <img src="https://wonderfulengineering.com/wp-content/uploads/2016/01/Desktop-Background-8.jpg"/>
            </div>
            <div class="img-container">
                <img src="https://wonderfulengineering.com/wp-content/uploads/2016/01/Desktop-Background-8.jpg"/>
            </div>
        </div>
    </div>


    <style>
        .slider-frame {
            overflow: hidden; 
            height: 800px;
            width: 1200px;
            margin-left: 0px;
            margin-top: 20px;
        }
        /*--------------------------------------------SLIDE ANIMATION------------------------------------------------------------------------------------------*/
        @-webkit-keyframes slide-animation{
            0% {left:0px;}
            10% {left:0px;}
            20% {left:1200px;}
            30% {left:1200px;}
            40% {left:2400px;}
            50% {left:2400px;}
            60% {left:1200px;}
            70% {left:1200px;}
            80% {left: 0px;}
            90% {left: 0px;}
            100% {left: 0px;}
        }
        .slider-images {
            width: 3600px;
            height: 800px;
            position: relative;
            margin: 0 0 0 -2400px;
            -webkit-animation-name: slide-animation;
            -webkit-animation-duration: 33s;
            -webkit-animation-iteration-count: infinite;
            -webkit-animation-play-state: running;
        }
        .img-container {
            height: 800px;
            width: 1200px;
            position: relative;
            float: left; 
        }
    </style>






























</asp:Content>

