import { createContext, useState } from "react";

export type MsgViewContextType = {
  isVisiable: boolean;
  changeVisibility: (state: boolean) => void;
};

const DefaultMsgViewState = {
  isVisiable: false,
  changeVisibility: () => {},
};

const MsgViewContext = createContext<MsgViewContextType>(DefaultMsgViewState);

function MsgViewContextProvider(props: {
  children: JSX.Element | JSX.Element[];
}) {
  const [isVisiable, setIsVisiable] = useState<boolean>(
    DefaultMsgViewState.isVisiable
  );
  function changeVisibility(state: boolean) {
    setIsVisiable(state);
  }

  return (
    <MsgViewContext.Provider value={{ isVisiable, changeVisibility }}>
      {props.children}
    </MsgViewContext.Provider>
  );
}

export { MsgViewContextProvider, MsgViewContext };
