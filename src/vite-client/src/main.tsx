import React from "react";
import ReactDOM from "react-dom/client";
import track from "react-tracking";
import { App } from "./App";

declare global {
  interface Window {
    trackingLayer: any;
    accountLayer: string;
  }
}

const TrackedApp = track(
  // app-level tracking data
  { app: "my-app" },

  // top-level options
  {
    // custom dispatch to console.log in addition to pushing to dataLayer[]
    dispatch: (data: any) => {
      console.log(data);
      (window.trackingLayer = window.trackingLayer || []).push(data);
    },
  }
)(App);

ReactDOM.createRoot(document.getElementById("root") as HTMLElement).render(
  <>
    <TrackedApp />
  </>
);
