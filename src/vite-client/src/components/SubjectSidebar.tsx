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

  const [counter, setCounter] = useState<number>(0);

  function getCounter() {
    setCounter(counter + 1);
    return counter;
  }

  function CreateCheckbox(subjectInfo: any): JSX.Element {
    return (
      <div>
        {subjectInfo["subSubjects"] != undefined
          ? subjectInfo["subSubjects"].map((subject: any) => {
              CreateCheckbox(subject);
            })
          : () => {
              console.log(getCounter());
              return (
                <Checkbox
                  key={getCounter()}
                  margin={1}
                  isChecked={allChecked}
                  isIndeterminate={isIndeterminate}
                  onChange={(e) =>
                    setCheckedItems([e.target.checked, e.target.checked])
                  }
                >
                  {subjectInfo["name"]}
                </Checkbox>
              );
            }}
      </div>
    );
  }

  const [subjectHierarchy, setSubjectHierarchy] = useState<any[]>([]);

  function getSubjectHierarchy() {
    fetch("/Subjects")
      .then((res: any) => res.json())
      .then((data) => {
        setSubjectHierarchy(data);
      });
  }

  useEffect(() => {
    getSubjectHierarchy();
  }, [subjectHierarchy.length != 0]);

  const [checkedItems, setCheckedItems] = useState<boolean[]>([]);

  const allChecked = checkedItems.every(Boolean);
  const isIndeterminate = checkedItems.some(Boolean) && !allChecked;

  return (
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
            {subjectHierarchy.map((subject, index) => {
              return <div key={index}>{CreateCheckbox(subject)}</div>;
            })}
          </Checkbox>
        </Card>
      </Card>
    </VStack>
  );
}

export { SubjectSidebar };
