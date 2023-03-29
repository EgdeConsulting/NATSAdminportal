import { createContext, useState } from "react";

export interface IMsg {
  sequenceNumber: string;
  stream: string;
  subject: string;
  timestamp?: string;
}

export interface IPayload {
  data: string;
}

export interface ISpecificMsg {
  headers: IHeaderProps[];
  payload: IPayload;
  subject: string;
}
const DefaultMsgViewState = {
  isVisible: false,
  changeVisibility: () => {},
};

export interface IHeaderProps {
  [key: string]: string;
  name: string;
  value: string;
}

export type MsgContextType = {
  currentMsg: IMsg;
  changeCurrentMsg: (msg: IMsg) => void;
};

const DefaultMsgState = {
  currentMsg: { sequenceNumber: "", stream: "", subject: "" },
  changeCurrentMsg: () => {},
};

const MsgContext = createContext<MsgContextType>(DefaultMsgState);

function MsgContextProvider(props: { children: JSX.Element | JSX.Element[] }) {
  const [currentMsg, setCurrentMsg] = useState<IMsg>(
    DefaultMsgState.currentMsg
  );
  function changeCurrentMsg(newMsg: IMsg) {
    setCurrentMsg(newMsg);
  }

  return (
    <MsgContext.Provider value={{ currentMsg, changeCurrentMsg }}>
      {props.children}
    </MsgContext.Provider>
  );
}

export { DefaultMsgState, MsgContext, MsgContextProvider };
