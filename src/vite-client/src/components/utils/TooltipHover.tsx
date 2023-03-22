import {
  PlacementWithLogical,
  ResponsiveValue,
  Tooltip,
} from "@chakra-ui/react";

function TooltipHover(props: {
  label: string;
  children: JSX.Element | JSX.Element[];
  placement?: PlacementWithLogical;
}) {
  return (
    <Tooltip
      hasArrow
      aria-label={props.label}
      label={props.label}
      placement={props.placement ? props.placement : "bottom"}
    >
      <span>{props.children}</span>
    </Tooltip>
  );
}

export { TooltipHover };
