import { createContext, useState } from "react";

export type StreamViewContextType = {
  isVisible: boolean;
  changeVisibility: (state: boolean) => void;
};

const DefaultStreamViewState = {
  isVisible: false,
  changeVisibility: () => {},
};

const StreamViewContext = createContext<StreamViewContextType>(
  DefaultStreamViewState
);

function StreamViewContextProvider(props: {
  children: JSX.Element | JSX.Element[];
}) {
  const [isVisible, setIsVisible] = useState<boolean>(
    DefaultStreamViewState.isVisible
  );
  function changeVisibility(state: boolean) {
    setIsVisible(state);
  }

  return (
    <StreamViewContext.Provider
      value={{ isVisible: isVisible, changeVisibility }}
    >
      {props.children}
    </StreamViewContext.Provider>
  );
}

export { StreamViewContextProvider, StreamViewContext };
