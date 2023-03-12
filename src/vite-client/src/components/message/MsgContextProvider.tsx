import { useState } from "react";
import { MsgContext, IMsg as IMsg, DefaultMsgState } from "components";

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

export { MsgContextProvider };
