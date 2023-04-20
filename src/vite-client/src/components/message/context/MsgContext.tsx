import { createContext, useState } from "react";

export interface MsgProps {
  sequenceNumber: string;
  stream: string;
  subject: string;
  timestamp?: string;
}

export interface PayloadProps {
  data: string;
}

export interface SpecificMsgProps {
  headers: HeaderProps[];
  payload: PayloadProps;
  subject: string;
}
const DefaultMsgViewState = {
  isVisible: false,
  changeVisibility: () => {},
};

export interface HeaderProps {
  [key: string]: string;
  name: string;
  value: string;
}

export type MsgContextType = {
  currentMsg: MsgProps;
  changeCurrentMsg: (msg: MsgProps) => void;
};

const DefaultMsgState = {
  currentMsg: { sequenceNumber: "", stream: "", subject: "" },
  changeCurrentMsg: () => {},
};

const MsgContext = createContext<MsgContextType>(DefaultMsgState);

function MsgContextProvider(props: { children: JSX.Element | JSX.Element[] }) {
  const [currentMsg, setCurrentMsg] = useState<MsgProps>(
    DefaultMsgState.currentMsg
  );
  function changeCurrentMsg(newMsg: MsgProps) {
    setCurrentMsg(newMsg);
  }

  return (
    <MsgContext.Provider value={{ currentMsg, changeCurrentMsg }}>
      {props.children}
    </MsgContext.Provider>
  );
}

export { DefaultMsgState, MsgContext, MsgContextProvider };
