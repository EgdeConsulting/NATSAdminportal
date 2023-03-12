import { createContext } from "react";

export interface IMsg {
  sequenceNumber: string;
  stream: string;
  subject: string;
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

export { DefaultMsgState, MsgContext };
