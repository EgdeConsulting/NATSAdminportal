import {
  Checkbox,
  CheckboxGroup,
  Drawer,
  Flex,
  Spacer,
  Box,
  Divider,
  Card,
  CardHeader,
  CardBody,
  Stack,
  VStack,
  Heading,
} from "@chakra-ui/react";
import { useState, useEffect } from "react";

function SubjectSidebar() {
  const [subjects, setSubjects] = useState<[]>([]);

  useEffect(() => {
    getSubjects();
  }, [subjects.length != 0]);

  function getSubjects() {
    fetch("/Subjects")
      .then((res) => res.json())
      .then((data) => {
        setSubjects(data);
        console.log(subjects);
      });
  }

  /////////////////////STREAM RELATED////////////////////////////
  // const [streamNames, setStreamNames] = useState<[]>([]);

  // useEffect(() => {
  //   getStreamNames(); //Denna står å går heila tio men finne jaffal rett amount of streams, men mest sannsynlig for performance heavy
  //   console.log(streamNames);
  // }, []);

  // function getStreamNames() {
  //   fetch("/StreamInfo")
  //     .then((res) => res.json())
  //     .then((data) => {
  //       setStreamNames(data);
  //     });
  // }

  const [checkedItems, setCheckedItems] = useState<boolean[]>([]);

  const allChecked = checkedItems.every(Boolean);
  const isIndeterminate = checkedItems.some(Boolean) && !allChecked;

  return (
    <div>
      <VStack margin={2} w={"100%"} h={"100%"}>
        <Card variant={"filled"} h={"100%"} w={"100%"}>
          <CardHeader>
            <Heading size={"md"}>Subject Hierarchy</Heading>
          </CardHeader>
          <Card margin={1} variant={"outline"}>
            <Checkbox
              margin={1}
              isChecked={allChecked}
              isIndeterminate={isIndeterminate}
              onChange={(e) =>
                setCheckedItems([e.target.checked, e.target.checked])
              }
            >
              {}
            </Checkbox>
          </Card>
        </Card>
      </VStack>
    </div>
  );
}

export { SubjectSidebar };
