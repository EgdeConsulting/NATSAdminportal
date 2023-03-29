import { IHeaderProps } from "components";
import { createContext, useState } from "react";

export type MsgViewContextType = {
  isVisible: boolean;
  changeVisibility: (state: boolean) => void;
};

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

const MsgViewContext = createContext<MsgViewContextType>(DefaultMsgViewState);

function MsgViewContextProvider(props: {
  children: JSX.Element | JSX.Element[];
}) {
  const [isVisible, setIsVisible] = useState<boolean>(
    DefaultMsgViewState.isVisible
  );
  function changeVisibility(state: boolean) {
    setIsVisible(state);
  }

  return (
    <MsgViewContext.Provider value={{ isVisible: isVisible, changeVisibility }}>
      {props.children}
    </MsgViewContext.Provider>
  );
}

export { MsgViewContextProvider, MsgViewContext };
